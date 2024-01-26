using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Normandy.Infrastructure.HttpClient
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpClientCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">handle填了,但是key没填</exception>
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IList<HttpClientOptions> options)
        {
            foreach (var option in options)
            {
                if (string.IsNullOrEmpty(option.Key))
                {
                    throw new ArgumentNullException(nameof(option.Key));
                }

                if (option?.Handler == null)
                {
                    services.AddHttpClient(option.Key, ConfigureHttpClient(option));
                    continue;
                }

                services.AddHttpClient(option.Key, ConfigureHttpClient(option)).ConfigurePrimaryHttpMessageHandler(() => option.Handler);
            }

            return services;
        }

        private static Action<System.Net.Http.HttpClient> ConfigureHttpClient(HttpClientOptions option)
        {
            if(option.Headers == null || option.Headers.Count == 0)
            {
                return c => { };
            }

            return c =>
            {
                foreach(var pair in option.Headers)
                {
                    c.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                }              
            };
        }
    }
}
