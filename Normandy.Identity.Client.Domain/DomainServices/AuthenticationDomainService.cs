using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Client.Domain.Requests;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Infrastructure.Util.HttpUtil;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Normandy.Identity.Client.Domain.Responses;
using Normandy.Identity.Client.Domain.Extensions;

namespace Normandy.Identity.Client.Domain.DomainServices
{
    public class AuthenticationDomainService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="cache"></param>
        public AuthenticationDomainService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient(ConstKeys.HttpClientKey);
            this.cache = cache;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response<SecuritySSOLoginResponse>> SSOLogin(SecuritySSOLoginRequest request)
        {
            var uri = CommonExtensions.GetSecurityUri(cache, configuration, ConstKeys.SecuritySSOLoginRoute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            return httpClient.RequestAsync<Response<SecuritySSOLoginResponse>>(message);
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <returns></returns>
        public Task<Response<string>> GetPublicKey()
        {
            var uri = CommonExtensions.GetSecurityUri(cache, configuration, ConstKeys.SecurityGetPublicKeyRoute);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            return httpClient.RequestAsync<Response<string>>(message);
        }
    }
}
