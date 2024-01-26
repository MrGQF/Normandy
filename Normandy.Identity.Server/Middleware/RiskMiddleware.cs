using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.Server.Application.Contracts.Requests;
using Normandy.Identity.Server.Application.Contracts.Responses;
using Normandy.Identity.Server.Application.Services.Risk;
using Normandy.Identity.Sever.Application.Services.Risk;
using Normandy.Infrastructure.Util.HttpUtil;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Middleware
{
    public class RiskMiddleware
    {
        public const string LoginPath = "/connect/token";
        private readonly RequestDelegate _next;
        private readonly IConfiguration configuration;
        private readonly NormandyIdentityOptions config = new NormandyIdentityOptions();

        private readonly IDictionary<string, RiskProcessorBase> riskInstancePairs = new Dictionary<string, RiskProcessorBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RiskMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="configuration"></param>
        /// <param name="pwdRiskService"></param>
        public RiskMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            PwdLoginRiskProcessor pwdRiskService)
        {
            _next = next;
            this.configuration = configuration;
            riskInstancePairs.TryAdd(LoginPath, pwdRiskService);
        }

        public async Task Invoke(HttpContext context)
        {
            configuration.Bind(config);

            var result = Selector(context);
            if (!result.isCheck)
            {
                await _next(context);
                return;
            }

            await CheckAsync(context, result.processor);
        }

        /// <summary>
        /// 风控处置选择器
        /// </summary>
        private (bool isCheck, RiskProcessorBase processor) Selector(HttpContext context)
        {
            if (!config.RiskSwitch)
            {
                return default;
            }

            var path = context.Request.Path.ToString().ToLower();
            if (riskInstancePairs.TryGetValue(path, out var processor))
            {
                return (true, processor);
            }

            return default;
        }

        /// <summary>
        /// 风控校验
        /// </summary>
        /// <param name="context"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        private async Task CheckAsync(HttpContext context, RiskProcessorBase processor)
        {
            var resBody = string.Empty;
            var result = new RiskResult<RiskEventInfo>();

            //read request body  
            context.Request.EnableBuffering();
            using var requestReader = new StreamReader(context.Request.Body);
            var reqBody = await requestReader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            try
            {
                // BeforeHandler
                await processor.BeforeHandler(context, result, reqBody);
                if (result.Disposed)
                {
                    var response = new Response<IList<RiskDisposeInfo>>
                    {
                        Code = (int)NormandyIdentityErrorCodes.RiskDisposed,
                        Message = NormandyIdentityErrorCodes.RiskDisposed.ToString(),
                        Data = result.DisposeInfo
                    };
                    await context.WriteResultAsync(response, HttpStatusCode.Forbidden).ConfigureAwait(false);
                    return;
                }

                // redefine response body
                var responseOriginalBody = context.Response.Body;
                using var memStream = new MemoryStream();
                context.Response.Body = memStream;

                // next
                await _next(context);

                // read response body
                memStream.Position = 0;
                var responseReader = new StreamReader(memStream);
                resBody = await responseReader.ReadToEndAsync();

                // InnerHandler
                await processor.InnerHandler(context, result, resBody);
                if (result.Disposed)
                {
                    var response = new Response<IList<RiskDisposeInfo>>
                    {
                        Code = (int)NormandyIdentityErrorCodes.RiskDisposed,
                        Message = NormandyIdentityErrorCodes.RiskDisposed.ToString(),
                        Data = result.DisposeInfo
                    };


                    memStream.SetLength(0);
                    await JsonSerializer.SerializeAsync(memStream, response);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }

                // copy response body
                memStream.Position = 0;
                await memStream.CopyToAsync(responseOriginalBody);
                context.Response.Body = responseOriginalBody;
            }
            finally
            {
                processor.AfterHandler(context, result);
            }
        }       
    }
}
