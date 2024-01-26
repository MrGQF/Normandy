using Normandy.Legacy.Client;
using Microsoft.Extensions.Caching.Memory;
using Normandy.Identity.Client.Authorization.Application.Contracts;
using Normandy.Identity.Client.Authorization.Application.Contracts.Requests;
using Normandy.Identity.Client.Authorization.Application.Contracts.Responses;
using Normandy.Identity.Client.Domain.DomainServices;
using Normandy.Identity.Client.Domain.Dtos;
using Normandy.Identity.Client.Domain.Extensions;
using Normandy.Identity.Client.Domain.Requests;
using Normandy.Identity.Client.Domain.Responses;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Identity.Client.Domain.Shared.Consts;
using Normandy.Identity.Client.Domain.Shared.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Normandy.Identity.Client.Authorization.Application
{
    /// <summary>
    /// 安全V2 授权业务
    /// </summary>
    public class SecurityAuthorization : IAuthorization
    {
        private readonly AuthorizationDomainService domainService;
        private readonly IMemoryCache cache;
        private readonly HttpClient httpClient;

        public SecurityAuthorization(
            AuthorizationDomainService domainService,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            this.domainService = domainService;
            this.cache = cache;
            this.httpClient = httpClientFactory.CreateClient(ConstKeys.HttpClientKey);
        }

        public Task<Result<string>> GetPassport(PassportRequest request)
        {
            return HandlerProcess(
               token => domainService.GetPassport(new PassportGetRequest 
               { 
                   Token = token?.Token,
                   QsId = request.QsId,
                   Product = request.Product,
                   Version = request.Version,
                   Imei = request.Imei,
                   Sdsn = request.Sdsn,
                   Securities = request.Securities,
                   Nohqlist = request.Nohqlist,
                   Newwgflag = request.Newwgflag,     
               }),
               a => a);
        }

        public Task<Result<CookieInfo>> GetCookie(CookieRequest request)
        {
            return HandlerProcess(
               token => domainService.GetCookie(new CookieGetRequest { Token = token?.Token, SignValid = request.SignValid }),
               response => MapperCookieInfo(response));
        }

        public async Task<Result<object>> LogOut()
        {
            var result = await HandlerProcess(
                token => domainService.LogOut(new LogoutRequest { Token = token?.Token, RefreshToken = token?.RefreshToken }),
                a => a);

            CommonExtensions.RemoveCache(cache);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private CookieInfo MapperCookieInfo(CookieGetResponse response)
        {
            return new CookieInfo
            {
                Domains = response?.Domains,
                Cookies = response?.Cookies
            };
        }

        /// <summary>
        /// 处理流程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private async Task<Result<TResult>> HandlerProcess<TResponse, TResult>(Func<TokenInfo, Task<Response<TResponse>>> handler, Func<TResponse, TResult> mapper)
        {
            var result = new Result<TResult> 
            {
                Flag = -1
            };
            try
            {
                var tokenInfo = await GetTokenAsync();

                var response = await handler?.Invoke(tokenInfo);


                if (response.Code == HttpCode.Ok)
                {
                    result.Flag = 0;
                    result.Data = mapper == null ? default : mapper.Invoke(response.Data);
                }

                result.Code = response.Code;
                result.StackTrace = response.Message;
                result.Message = response.Message;
                return result;
            }
            catch (Exception ex)
            {
                result.Code = HttpCode.InnerError;
                result.Message = ex.Message;
                result.StackTrace = ex.StackTrace;
            }

            return result;
        }

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <returns></returns>
        private Task<TokenInfo> GetTokenAsync()
        {
            return CommonExtensions.GetOrCreateTokenCacheAsync(cache,
                 async a =>
                 {
                     var response = await domainService.RefreshToken(new RefreshTokenRequest
                     {
                         RefreshToken = a
                     });

                     if (response?.Code != HttpCode.Ok)
                     {
                         throw new TokenRefreshFailedException(JsonSerializer.Serialize(response));
                     }

                     return response.Data?.TokenInfo;
                 });
        }


        public Task<CloudInfo> PollCloundData(string biz, string path, long version)
        {
            var tuple = CommonExtensions.GetUserCache(cache);
            var authInfo = new AuthInfo(
                tuple.userInfo.NickName,
            Convert.ToString(tuple.userInfo.UserId),
            tuple.sessionInfo.SessionId,
            tuple.sessionInfo.Expires,
            tuple.sessionInfo.Sign);

            return DataCenterHelper.PollCloundData(httpClient, authInfo, biz, path, version);
        }

        public Task<CloudInfo> PushCloundData(string biz, string path, long version, string Filename, byte[] file)
        {
            var tuple = CommonExtensions.GetUserCache(cache);
            var authInfo = new AuthInfo(
                tuple.userInfo.NickName,
            Convert.ToString(tuple.userInfo.UserId),
            tuple.sessionInfo.SessionId,
            tuple.sessionInfo.Expires,
            tuple.sessionInfo.Sign);

            return DataCenterHelper.PushCloundData(httpClient, authInfo, biz, path, version, Filename, file);
        }

        public Task<CloudInfo> DownloadSelfCodeDataAsync(string bizName, string path, long version, string clientType)
        {
            var tuple = CommonExtensions.GetUserCache(cache);
            var authInfo = new AuthInfo(
                tuple.userInfo.NickName,
            Convert.ToString(tuple.userInfo.UserId),
            tuple.sessionInfo.SessionId,
            tuple.sessionInfo.Expires,
            tuple.sessionInfo.Sign);

            return DataCenterHelper.DownloadSelfCodeDataAsync(httpClient, authInfo, bizName, path, version, clientType);
        }

        public Task<CloudInfo> PushSelfCodeToCloudAsync(string bizName, string path, long version, byte[] file, string clientType)
        {
            var tuple = CommonExtensions.GetUserCache(cache);
            var authInfo = new AuthInfo(
                tuple.userInfo.NickName,
            Convert.ToString(tuple.userInfo.UserId),
            tuple.sessionInfo.SessionId,
            tuple.sessionInfo.Expires,
            tuple.sessionInfo.Sign);

            return DataCenterHelper.PushSelfCodeToCloudAsync(httpClient, authInfo, bizName, path, version, file, clientType);
        }
    }
}
