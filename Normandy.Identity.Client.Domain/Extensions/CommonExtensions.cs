using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Client.Domain.Dtos;
using Normandy.Identity.Client.Domain.Responses;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Identity.Client.Domain.Shared.Consts;
using Normandy.Identity.Client.Domain.Shared.Exceptions;
using Normandy.Infrastructure.HttpClient;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Normandy.Identity.Client.Domain.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// 获取uri
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="configuration"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static Uri GetSecurityUri(IMemoryCache cache, IConfiguration configuration, string route)
        {
            return UriExtensions.GetSecurityUri(
                cache,
                () => new List<string>
                {
                    configuration.GetSection(ConstKeys.ConfigSecurityDoaminKey).Value,
                    configuration.GetSection(ConstKeys.ConfigSecurityHostKey).Value
                },
                ConstKeys.DnsErrorLimit,
                route);
        }

        /// <summary>
        /// 刷新令牌缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="info"></param>
        public static void RefreshTokenCache(IMemoryCache cache, TokenInfo info)
        {
            if (string.IsNullOrWhiteSpace(info?.RefreshToken)
                || string.IsNullOrWhiteSpace(info?.RefreshExpireIn)
                || string.IsNullOrWhiteSpace(info?.Token)
                || string.IsNullOrWhiteSpace(info?.ExpiresIn))
            {
                throw new TokenInfoNullException(JsonSerializer.Serialize(info), null);
            }

            // 保存TokenInfo
            var refreshExpire = DateTimeOffset.Now.AddSeconds(Convert.ToInt64(info.RefreshExpireIn));
            cache.Set(CacheKeys.CacheKeySecurityRefreshToken, info.RefreshToken, refreshExpire);
            var accessExpire = DateTimeOffset.Now.AddSeconds(Convert.ToInt64(info.ExpiresIn));
            cache.Set(CacheKeys.CacheKeySecurityAccessToken, info.Token, accessExpire);
        }

        /// <summary>
        /// 刷新session
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="info"></param>
        public static void RefreshSessionCache(IMemoryCache cache, SessionInfo info)
        {
            if (string.IsNullOrWhiteSpace(info?.Sign)
                || string.IsNullOrWhiteSpace(info?.SignTime)
                || string.IsNullOrWhiteSpace(info.Expires)
                || string.IsNullOrWhiteSpace(info.SessionId))
             {
                throw new SessionInfoNullException(JsonSerializer.Serialize(info), null);
            }

            // 保存session
            cache.Set(CacheKeys.CacheKeySessionInfo, JsonSerializer.Serialize(info));
        }

        /// <summary>
        /// 刷新用户信息
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="info"></param>
        public static void RefreshUserInfoCache(IMemoryCache cache, UserInfo info)
        {
            if (string.IsNullOrWhiteSpace(info?.NickName)
                || info.UserId == 0)
            {
                throw new UserInfoNullException(JsonSerializer.Serialize(info), null);
            }

            // 保存UserInfo
            cache.Set(CacheKeys.CacheKeyUserInfo, JsonSerializer.Serialize(info));
        }

        /// <summary>
        /// 获取或者创建令牌
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="createHandler"></param>
        /// <returns></returns>
        /// <exception cref="LoginExpiredException">登录过期</exception>
        public static async Task<TokenInfo> GetOrCreateTokenCacheAsync(IMemoryCache cache, Func<string, Task<TokenInfo>> createHandler)
        {
            if (!cache.TryGetValue(CacheKeys.CacheKeySecurityRefreshToken, out string refreshToken))
            {
                throw new LoginExpiredException(null, null);
            }

            if (cache.TryGetValue(CacheKeys.CacheKeySecurityAccessToken, out string token))
            {
                return new TokenInfo { Token = token, RefreshToken = refreshToken };
            }           

            var tokenInfo = await createHandler?.Invoke(refreshToken);     

            RefreshTokenCache(cache, tokenInfo);
            return tokenInfo;
        }

        /// <summary>
        /// 获取用户缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static (SessionInfo sessionInfo, UserInfo userInfo) GetUserCache(IMemoryCache cache)
        {
            var sessionInfo = new SessionInfo();
            var userInfo = new UserInfo();
            if (cache.TryGetValue(CacheKeys.CacheKeyUserInfo, out string userInfoJson))
            {
                userInfo = JsonSerializer.Deserialize<UserInfo>(userInfoJson);
            }

            if (cache.TryGetValue(CacheKeys.CacheKeySessionInfo, out string sessionInfoJson))
            {
                sessionInfo = JsonSerializer.Deserialize<SessionInfo>(sessionInfoJson);
            }

            return (sessionInfo, userInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static void RemoveCache(IMemoryCache cache)
        {
            cache.Remove(CacheKeys.CacheKeySecurityRefreshToken);
            cache.Remove(CacheKeys.CacheKeySecurityAccessToken);
            cache.Remove(CacheKeys.CacheKeySessionInfo);
            cache.Remove(CacheKeys.CacheKeyUserInfo);
        }
    }
}
