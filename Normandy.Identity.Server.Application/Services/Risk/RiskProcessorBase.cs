using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Domain.Shared.Enums;
using Normandy.Identity.Domain.Shared.Exceptions;
using Normandy.Identity.Server.Application.Contracts;
using Normandy.Identity.Server.Application.Contracts.Dtos;
using Normandy.Identity.Server.Application.Contracts.Requests;
using Normandy.Identity.Server.Application.Contracts.Responses;
using Normandy.Infrastructure.Util.Common;
using Normandy.Infrastructure.Util.HttpUtil;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Normandy.Identity.Sever.Application.Services.Risk
{
    public abstract class RiskProcessorBase : IRiskService<RiskEventInfo>
    {
        public abstract int BeforeEventModelId { get; }

        public abstract int InnerEventModelId { get; }

        public abstract int AfterEventModelId { get; }

        private readonly HttpClient httpClient;
        private readonly NormandyIdentityOptions config = new NormandyIdentityOptions();
        private readonly ILogger<RiskProcessorBase> logger;

        public RiskProcessorBase(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ILogger<RiskProcessorBase> logger)
        {
            configuration.Bind(config);

            this.logger = logger;

            httpClient = clientFactory.CreateClient(nameof(NormandyIdentityOptions.RiskCheckUrl));
            httpClient.Timeout = TimeSpan.FromMilliseconds(config.RiskHttpTimeOutMilliseconds);
        }

        /// <summary>
        /// 事前处置
        /// </summary>
        public Task BeforeHandler(HttpContext context, in RiskResult<RiskEventInfo> result, string reqBody)
        {
            return Process(
                context,
                result,
                reqBody,
                string.Empty,
                result =>
                {
                    if (string.IsNullOrWhiteSpace(reqBody))
                    {
                        return;
                    }

                    var identityHeader = ParseHeaders(context);
                    var data = ParseRequest(context, identityHeader, reqBody);
                    result.Request = new RiskRequest<RiskEventInfo>
                    {
                        ModelId = BeforeEventModelId,
                        Sync = true,
                        Data = data
                    };
                });
        }


        // <summary>
        /// 事前处置
        /// </summary>
        public Task InnerHandler(HttpContext context, in RiskResult<RiskEventInfo> result, string resBody)
        {
            return Process(
                context,
                result,
                string.Empty,
                resBody,
                result =>
                {
                    if (result.Request == null
                    || result.Request.Data == null)
                    {
                        return;
                    }

                    result.Request.ModelId = InnerEventModelId;
                    result.Request.Data.StatusCode = Convert.ToString(context.Response.StatusCode);
                    if (string.IsNullOrWhiteSpace(resBody)
                    || context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
                    {
                        return;
                    }

                    ParseResponse(result.Request.Data, resBody);
                });
        }

        // <summary>
        /// 事前处置
        /// </summary>
        public Task AfterHandler(HttpContext context, in RiskResult<RiskEventInfo> result)
        {
            return Process(
                context,
                result,
                string.Empty,
                string.Empty,
                result =>
                {
                    if (result.Request == null
                    || result.Request.Data == null)
                    {
                        return;
                    }

                    result.Request.ModelId = AfterEventModelId;
                    if (result.Disposed)
                    {
                        result.Request.Data.Code = Convert.ToString((int)NormandyIdentityErrorCodes.RiskDisposed);
                        result.Request.Data.StatusCode = Convert.ToString(context.Response.StatusCode);
                    }
                    result.Request.Sync = false;
                });
        }

        /// <summary>
        /// 解析 请求值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identityHeader"></param>
        /// <param name="reqBody"></param>
        /// <returns></returns>
        public abstract RiskEventInfo ParseRequest(
            HttpContext context,
            IdentityHeader identityHeader,
            string reqBody);

        // <summary>
        /// 解析 返回值
        /// </summary>
        public abstract void ParseResponse(RiskEventInfo data, string resBody);

        /// <summary>
        /// 解析 请求头
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IdentityHeader ParseHeaders(HttpContext context)
        {
            var headerDic = context.Request.ParseHeaders();
            return headerDic.ConvertToModel<IdentityHeader>();
        }

        private async Task Process<T>(
            HttpContext context,
            RiskResult<T> result,
            string reqBody,
            string resBody,
            Action<RiskResult<T>> resultHandler)
        {
            try
            {
                if (result == null)
                {
                    return;
                }
                resultHandler?.Invoke(result);
                await Upload(result);
            }
            catch (Exception ex)
            {
                var msg = $"Type: {ex.GetType()}; Msg:{ex.Message}; StackTrace:{ex.StackTrace}";
                logger.LogError("ErrorMessage:{ErrorMessage} Request:{Request} Response:{Response}", msg, reqBody, resBody);
                return;
            }
        }

        /// <summary>
        /// 风控事件上传接口
        /// </summary>
        /// <param name="result"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RiskCheckUrlEmptyException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        private async Task Upload<T>(RiskResult<T> result)
        {
            if (result == null
                || result.Request == null
                || result.Request.Data == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(config.RiskCheckUrl))
            {
                throw new RiskCheckUrlEmptyException();
            }

            var reqDic = result.Request.ToDictionary();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, config.RiskCheckUrl)
            {
                Content = new FormUrlEncodedContent(reqDic)
            };
            var res = await httpClient.RequestAsync<RiskResponse<object>>(requestMessage);
            if (res.Data != null)
            {
                result.Disposed = true;
                result.DisposeInfo = res.Data.ConvertToModel<List<RiskDisposeInfo>>();
            }

            return;
        }
    }
}
