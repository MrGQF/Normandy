using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Client.Domain.Extensions;
using Normandy.Identity.Client.Domain.Requests;
using Normandy.Identity.Client.Domain.Responses;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Infrastructure.Util.HttpUtil;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Domain.DomainServices
{
    public class AuthorizationDomainService
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
        public AuthorizationDomainService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            this.configuration = configuration;
            this.httpClient = httpClientFactory.CreateClient(ConstKeys.HttpClientKey);
            this.cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var uri = CommonExtensions.GetSecurityUri(cache, configuration, ConstKeys.SecurityRefreshTokenRoute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            return httpClient.RequestAsync<Response<RefreshTokenResponse>>(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response<object>> LogOut(LogoutRequest request)
        {
            var uri = CommonExtensions.GetSecurityUri(cache, configuration, ConstKeys.SecurityLogoutRoute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            return httpClient.RequestAsync<Response<object>>(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response<string>> GetPassport(PassportGetRequest request)
        {
            var uri = CommonExtensions.GetSecurityUri(cache, configuration, ConstKeys.SecurityGetPassportRoute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            return httpClient.RequestAsync<Response<string>>(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response<CookieGetResponse>> GetCookie(CookieGetRequest request)
        {
            var uri = CommonExtensions.GetSecurityUri(cache, configuration, ConstKeys.SecurityGetCookieRoute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request)
            };

            return httpClient.RequestAsync<Response<CookieGetResponse>>(message);
        }        
    }
}
