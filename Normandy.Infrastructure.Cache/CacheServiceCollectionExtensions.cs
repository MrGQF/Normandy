using Microsoft.Extensions.DependencyInjection;

namespace Normandy.Infrastructure.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemoryCaches(this IServiceCollection services)
        {
            services.AddMemoryCache();

            return services;
        }
    }
}
