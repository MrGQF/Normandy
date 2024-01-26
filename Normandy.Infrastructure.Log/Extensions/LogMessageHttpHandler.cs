using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Log.Extensions
{
    /// <summary>
    /// 日志记录消息 
    /// 配置文件
    /// </summary>
    public class LogMessageHttpHandler : DelegatingHandler
    {
        private const string SwitchKey = "OpenTraceInLogMessageHttpHandler";
        private const string CorrelationIdKey = "x-correlation-id";

        private readonly IHttpContextAccessor accessor;
        private readonly ILogger<LogMessageHttpHandler> logger;
        private readonly IConfiguration configuration;

        public LogMessageHttpHandler(
            IHttpContextAccessor accessor,
            ILogger<LogMessageHttpHandler> logger,
            IConfiguration configuration)
        {
            this.accessor = accessor;
            this.logger = logger;
            this.configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var watch = new Stopwatch();
            var traceId = HttpContextAccessorExtensions.GetCorrelationId(accessor, CorrelationIdKey);
            request.Headers.Add(CorrelationIdKey, traceId);

            Exception err = default;
            HttpResponseMessage responseMessage = default;
            try
            {
                watch.Start();
                responseMessage = await base.SendAsync(request, cancellationToken);
                return responseMessage;
            }
            catch (Exception ex)
            {
                err = ex;
                throw;
            }
            finally
            {
                watch.Stop();
                await Trace(err, request, responseMessage, watch.ElapsedMilliseconds, traceId);
            }
        }

        private async Task Trace(Exception ex, HttpRequestMessage requestMsg, HttpResponseMessage responseMsg, long ElapsedMilliseconds, string traceId)
        {
            var isTrace = configuration.GetValue<bool>(SwitchKey);
            if (!isTrace)
            {
                return;
            }

            var Url = requestMsg?.RequestUri.ToString();
            var Path = requestMsg?.RequestUri.AbsolutePath;
            var CorrelationId = traceId;
            var Method = requestMsg?.Method.ToString();

            var Request = requestMsg?.Content == null ? string.Empty : await requestMsg.Content.ReadAsStringAsync();
            var Response = responseMsg?.Content == null ? string.Empty : await responseMsg.Content.ReadAsStringAsync();
            var StatusCode = responseMsg?.StatusCode.ToString();
            var ErrorMsg = ex == null ? string.Empty : $"Type: {ex.GetType()}; Msg:{ex.Message}; StackTrace:{ex.StackTrace}";

            logger.LogInformation("Url:{Url} Path:{Path} ElapsedMilliseconds:{ElapsedMilliseconds}; CorrelationId:{CorrelationId}; Method:{Method}; Request:{Request}; Response:{Response}; StatusCode{StatusCode}; ErrorMsg:{ErrorMsg}",
                Url, Path, ElapsedMilliseconds, CorrelationId, Method, Request, Response, StatusCode, ErrorMsg);
        }
    }
}
