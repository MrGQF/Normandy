using Microsoft.Extensions.DependencyInjection;
using Normandy.Identity.Client.Authorization.Application.Contracts;
using Normandy.Identity.Client.Domain.DomainServices;
using Normandy.Identity.Client.Domain.Shared;
using System;
namespace Normandy.Identity.Client.Authorization.Application
{
    public static class AuthorizationServiceCollectionExtensions
    {
        /// <summary>
        /// 注入授权业务实例
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizations(this IServiceCollection services)
        {
            services.AddSingleton(implementationFactory =>
            {
                Func<AuthType, IAuthorization> accesor = type =>
                {
                    switch (type)
                    {
                        case AuthType.Security:
                            return implementationFactory.GetService<SecurityAuthorization>();
                        case AuthType.AuthCenter:
                            return implementationFactory.GetService<AuthCenterAuthorization>();
                        default:
                            throw new NotSupportedException();
                    }
                };
                return accesor;
            });

            services.AddSingleton<SecurityAuthorization>();
            services.AddSingleton<AuthCenterAuthorization>();

            // todo auto add DomainService By assembly
            services.AddSingleton<AuthorizationDomainService>();

            return services;
        }
    }
}
