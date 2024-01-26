using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Reflection;

namespace Normandy.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 继承自定义泛型接口自动注入。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AutoDI(this IServiceCollection services, params Assembly[] assemblies)
        {
            var baseType = typeof(IAutoDIable);
            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes().Where(x => x.IsClass && typeof(IAutoDIable).IsAssignableFrom(x));
                foreach (var type in handlerTypes)
                {
                    var interfaces = type.GetInterfaces();

                    #region 获取生命周期
                    var lifetime = ServiceLifetime.Scoped;
                    var lifetimeInterface = interfaces.FirstOrDefault(x => x.FullName != baseType.FullName);
                    if (lifetimeInterface != null)
                    {
                        switch (lifetimeInterface.Name)
                        {
                            case nameof(IScopedAutoDIable):
                                lifetime = ServiceLifetime.Scoped;
                                break;
                            case nameof(ITransientAutoDIable):
                                lifetime = ServiceLifetime.Transient;
                                break;
                            case nameof(ISingletonAutoDIable):
                                lifetime = ServiceLifetime.Singleton;
                                break;
                        }
                    }
                    #endregion
                    var interfaceType = interfaces.FirstOrDefault(x => x.Name == $"I{type.Name}");
                    if (interfaceType == null)
                    {
                        interfaceType = type;
                    }
                    var serviceDescriptor = new ServiceDescriptor(interfaceType, type, lifetime);
                    if (!services.Contains(serviceDescriptor))
                    {
                        services.TryAdd(serviceDescriptor);
                    }
                }
            }
            return services;
        }
    }
}
