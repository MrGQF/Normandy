using Microsoft.Extensions.DependencyInjection;

namespace Normandy.Infrastructure.Config
{
    public static class ProtectionConfigCollectionExtensions
    {
        public static IServiceCollection AddProtection(this IServiceCollection services)
        {
            services.AddDataProtection();
            return services;
        }
    }
}
