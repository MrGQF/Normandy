using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Normandy.Infrastructure.Util.HttpUtil;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Util.Middleware
{
    public class ApiExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ApiExceptionMiddleWare> logger;

        public ApiExceptionMiddleWare(
            RequestDelegate next,
            ILogger<ApiExceptionMiddleWare> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
                var features = context.Features;
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            try
            {
                var path = context.Request.Path;
                LogException(ex, context.Request, path);
              
                var returnError = ex.InnerException == null
                    ? ex
                    : ex.InnerException;
                if (returnError is AggregateException)
                {
                    var aggreEx = returnError as AggregateException;
                    returnError = aggreEx.InnerExceptions.First();
                }

                // 构造返回值
                int code;
                var statusCode = HttpStatusCode.OK;
                if (ex is ApplicationException)
                {
                    code = -1;

                }
                else
                {
                    code = -500;
                }
                await context.WriteResultAsync(
                    new 
                    {
                        flag = -1,
                        Code = code,
                        Message = ex.Message,
                        stackTrace = ex.StackTrace
                    }, 
                    statusCode).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError("ApiResponseMiddleWare Error:", e);
            }
        }

        /// <summary>
        /// 递归解析AggregateException,记录日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="request"></param>
        /// <param name="path"></param>
        private void LogException(Exception ex, HttpRequest request, string path)
        {
            if (ex == null)
            {
                return;
            }

            var aggregateException = ex as AggregateException;
            if (aggregateException?.InnerExceptions != null)
            {
                foreach (var iEx in aggregateException.InnerExceptions)
                {
                    LogException(iEx, request, path);
                }

                return;
            }

            logger.LogInformation($"ApiResponseMiddleWare.Handle Url:{request.GetDisplayUrl()} \t\t,Paht:{path}", ex);
        }       
    }
}
