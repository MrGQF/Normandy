using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Normandy.Infrastructure.Util.HttpUtil
{
    public static class HttpContextExtensions
    {
        ///<summary>
        ///获取客户端IP地址
        ///</summary>
        ///<returns>IP地址</returns>
        public static string GetClientIPAddr(this HttpContext httpContext)
        {
            object obj;
            if (httpContext == null)
            {
                obj = null;
            }
            else
            {
                HttpRequest request = httpContext!.Request;
                obj = ((request != null) ? request.Headers["X-forwarded-for"].FirstOrDefault() : null);
            }

            string text = (string)obj;
            if (!string.IsNullOrEmpty(text))
            {
                return GetIpAddressFromProxy(text);
            }

            return httpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        private static string GetIpAddressFromProxy(string proxifiedIpList)
        {
            string[] array = proxifiedIpList.Split(',');
            if (array.Length != 0)
            {
                if (!array[0].Contains(":"))
                {
                    return array[0];
                }

                return array[0].Substring(0, array[0].LastIndexOf(":", StringComparison.Ordinal));
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="value"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static async Task WriteResultAsync(this HttpContext httpContext, object value, HttpStatusCode statusCode)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var objectResult =
                new ObjectResult(value) { StatusCode = (int)statusCode };
            var actionContext = new ActionContext { HttpContext = httpContext };
            await objectResult.ExecuteResultAsync(actionContext).ConfigureAwait(false);
        }
    }
}
