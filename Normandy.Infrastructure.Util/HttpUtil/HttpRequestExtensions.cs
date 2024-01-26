using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Normandy.Infrastructure.Util.HttpUtil
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Get Header's value from request message with out any process
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headerKey">Header's key</param>
        /// <returns>If not exist, return null</returns>
        public static string GetHeaderValueRaw(this HttpRequest request, string headerKey)
        {
            request.Headers.TryGetValue(headerKey, out var values);
            return values.FirstOrDefault();
        }

        /// <summary>
        /// Get Header's value from request message then HttpUtility.UrlDecode
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headerKey">Header's key</param>
        /// <returns>If not exists, return null</returns>
        public static string GetHeaderValueUrlDecoded(this HttpRequest request, string headerKey)
        {
            var headerValueRaw = GetHeaderValueRaw(request, headerKey);
            if (headerValueRaw == null)
                return null;

            var headerValueDecoded = WebUtility.UrlDecode(headerValueRaw);
            return headerValueDecoded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseHeaders(this HttpRequest request)
        {
            return request.Headers.Keys.Aggregate(new Dictionary<string, string>(), (des, key) => 
            {
                var val = request.GetHeaderValueUrlDecoded(key);
                des.Add(key, val);
                return des;
            });          
        }
    }   
}
