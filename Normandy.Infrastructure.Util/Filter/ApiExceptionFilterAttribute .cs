using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Util.Filter
{
    /// <summary>
    /// api异常统一处理过滤器
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> logger;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(ExceptionContext));
            }

            context.Result = BuildExceptionResult(context.Exception);
            base.OnException(context);
        }

        /// <summary>
        /// 包装处理异常格式
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ObjectResult BuildExceptionResult(Exception ex)
        {        
            LogException(ex);
            if (ex is AggregateException)
            {
                var aggreEx = ex as AggregateException;
                ex = aggreEx.InnerExceptions.First();
            }

            var flag = -1;
            var message = $"{ex.GetType().Name}\n{ex.Message}";
            var stackTrace = ex.StackTrace;
            int code;
            if (ex is ApplicationException)
            {
                var errorCode = ex.Data[nameof(ApplicationException)];
                code = errorCode == null ? -1 : (int)errorCode;
            }
            else
            {
                code = -500;
            }

            return new ObjectResult(new 
            { 
                Flag = flag, 
                Code = code, 
                Message = message, 
                StackTrace = stackTrace
            });
        }

        /// <summary>
        /// 递归解析AggregateException,记录日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="request"></param>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        private void LogException(Exception ex)
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
                    LogException(iEx);
                }

                return;
            }

            logger.LogInformation($"WebAPIException:", ex);
        }
    }
}
