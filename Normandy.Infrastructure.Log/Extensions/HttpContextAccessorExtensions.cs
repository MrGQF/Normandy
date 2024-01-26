using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace Normandy.Infrastructure.Log.Extensions
{
    public static class HttpContextAccessorExtensions
    {
        private const string IdentityHeaderKey_TraceId = "x-correlation-id";

        public static string GetCorrelationId(IHttpContextAccessor contextAccessor, string headerKey = IdentityHeaderKey_TraceId)
        {
            string text = string.Empty;
            if (contextAccessor == null)
            {
                return string.Empty;
            }

            if (contextAccessor.HttpContext!.Request.Headers.TryGetValue(headerKey, out StringValues value))
            {
                text = value.FirstOrDefault();
            }
            else if (contextAccessor.HttpContext!.Response.Headers.TryGetValue(headerKey, out value))
            {
                text = value.FirstOrDefault();
            }

            string text2 = string.IsNullOrEmpty(text) ? Guid.NewGuid().ToString() : text;
            if (!contextAccessor.HttpContext!.Response.Headers.ContainsKey(headerKey))
            {
                contextAccessor.HttpContext!.Response.Headers.Add(headerKey, text2);
            }

            return text2;
        }

        public static string GetCorrelationId(this HttpContext context, string headerKey = IdentityHeaderKey_TraceId)
        {
            string text = string.Empty;
            if (context == null)
            {
                return string.Empty;
            }

            if (context!.Request.Headers.TryGetValue(headerKey, out StringValues value))
            {
                text = value.FirstOrDefault();
            }
            else if (context!.Response.Headers.TryGetValue(headerKey, out value))
            {
                text = value.FirstOrDefault();
            }

            string text2 = string.IsNullOrEmpty(text) ? Guid.NewGuid().ToString() : text;
            if (!context!.Response.Headers.ContainsKey(headerKey))
            {
                context!.Response.Headers.Add(headerKey, text2);
            }

            return text2;
        }
    }
}
