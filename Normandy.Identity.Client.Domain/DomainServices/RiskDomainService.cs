using Microsoft.Extensions.Configuration;
using Normandy.Identity.Client.Domain.Requests;
using Normandy.Identity.Client.Domain.Shared;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace Normandy.Identity.Client.Domain.DomainServices
{
    /// <summary>
    /// 风控相关
    /// </summary>
    public class RiskDomainService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="cache"></param>
        public RiskDomainService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient(ConstKeys.HttpClientKey);
        }

        /// <summary>
        /// 风控结果上传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task RiskEventCheck(RiskEventCheckRequest request)
        {

            var uri = configuration.GetSection(ConstKeys.RiskEventCheckUriKey).Value;
            var formContent = new Dictionary<string, string>
            {
                { "eventModelId", request.EventModelId },
                { "sync", request.Sync.ToString().ToLower() },
                { "eventData", JsonSerializer.Serialize(request.EventData) },
            };
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new FormUrlEncodedContent(formContent)
            };

            _ = await httpClient.SendAsync(message);

            return;
        }
    }
}
