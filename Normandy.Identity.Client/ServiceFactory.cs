using Microsoft.Extensions.DependencyInjection;

namespace Normandy.Identity.Client
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ServiceFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ServiceCollection Services = new ServiceCollection();

        public static ServiceProvider Provider { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection SetServiceProvider(this IServiceCollection services)
        {
            if(Provider == null)
            {
                Provider = services.BuildServiceProvider();
            }

            return services;
        }
    }
}
