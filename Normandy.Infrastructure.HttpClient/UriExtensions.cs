using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Normandy.Infrastructure.HttpClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// 获取uri
        /// 通过缓存动态切换
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="configuration"></param>
        /// <param name="route"></param>
        /// <param name="addressHandler">当都不满足时，默认使用第一个</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Uri GetSecurityUri(IMemoryCache cache, Func<List<string>> addressHandler, int limit, string route = null)
        {
            var addressList = addressHandler?.Invoke();
            if(addressList == null || addressList.Count <=0)
            {
                throw new ArgumentNullException(nameof(addressHandler));
            }

            var baseUri = GetBaseUri(
                cache,
                addressList,
                limit);
            if(string.IsNullOrWhiteSpace(route))
            {
                return baseUri;
            }

            var relativeUri = new Uri(route, UriKind.Relative);
            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// 获取uri 解析结果统计缓存key
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string GetKey(string val)
        {
            return $"{nameof(UriExtensions)}{Convert.ToBase64String(Encoding.UTF8.GetBytes(val))}";
        }

        /// <summary>
        /// 获取Uri
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="uriList">可使用的uri列表，包括：域名和IP,优先级降级排列 </param>
        /// <param name="limit">解析错误限制次数;错误次数> limit 时，切换下一个备用地址:均不可用时,使用第一个</param>
        /// <returns></returns>
        private static Uri GetBaseUri(IMemoryCache cache, IList<string> addressList, int limit)
        {
            foreach (var address in addressList)
            {
                if (string.IsNullOrEmpty(address))
                {
                    continue;
                }

                var key = GetKey(address);
                if (!cache.TryGetValue(key, out var val))
                {
                    return new Uri(address);
                }

                if (Convert.ToInt32(val) <= limit)
                {
                    return new Uri(address);
                }
            }

            return null;
        }
    }
}
