using Microsoft.Extensions.DependencyInjection;
using Normandy.Identity.Client.Authentication.Application.Contracts;
using Normandy.Identity.Client.Domain.DomainServices;
using Normandy.Identity.Client.Domain.Shared;
using System;

namespace Normandy.Identity.Client.Authentication.Application
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthenticationServiceCollectionExtensions
    {
        /// <summary>
        /// 注入认证业务实例
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthentications(this IServiceCollection services)
        {
            services.AddSingleton(implementationFactory =>
            {
                Func<AuthType, IAuthentication> accesor = type =>
                {
                    switch (type)
                    {
                        case AuthType.Security:
                            return implementationFactory.GetService<SecurityAuthentication>();
                        case AuthType.AuthCenter:
                            return implementationFactory.GetService<AuthCenterAuthentication>();
                        default:
                            throw new NotSupportedException();
                    }
                };
                return accesor;
            });

            services.AddSingleton<SecurityAuthentication>();
            services.AddSingleton<AuthCenterAuthentication>();

            // todo auto add DomainService By assembly
            services.AddSingleton<AuthenticationDomainService>();
            services.AddSingleton<RiskDomainService>();

            return services;
        }
    }
}
