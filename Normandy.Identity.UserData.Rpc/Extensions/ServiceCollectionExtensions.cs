using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Identity.Domain.Shared;
using Normandy.Identity.UserData.EnityFrameworkCore.DbContexts;
using Normandy.Infrastructure.DI;
using Normandy.Infrastructure.EntityFrameworkCore.DependencyInjection;
using Normandy.Infrastructure.Log.Extensions;
using Normandy.Infrastructure.Log.Provider;
using Normandy.Infrastructure.Mapper;
using Normandy.Infrastructure.Util.Reflection;
using System;
using System.Linq;

namespace Normandy.Identity.UserData.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostBuilder webHostBuilder)
        {
            var config = new NormandyIdentityOptions();
            configuration.Bind(config);

            services.AddAutoMapper();

            services.AddGrpc();
            if (config.IsUseGrpcHttpApi)
            {
                services.AddGrpcHttpApi();
            }

            services.AddDbContexts(config);

            services.AddHttpClients(config);

            services.AddDI();

            webHostBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(config.UserDataRpcServerHttp2Port, o => o.Protocols =
                    HttpProtocols.Http2);

                options.ListenAnyIP(config.UserDataRpcServerHttp1Port, o => o.Protocols =
                    HttpProtocols.Http1);
            });

            return services;
        }

        public static IServiceCollection AddDbContexts(this IServiceCollection services, NormandyIdentityOptions config)
        {
            services
            .AddDbContext<UserDataContext>(builder => builder
            .UseMySql(config.UserDbConnectionstrings.First(), new MySqlServerVersion(new Version(5, 7, 20)))
            .EnableSensitiveDataLogging())
            .AddUnitOfWork<UserDataContext>();
            return services;
        }

        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            var assemblys = AppDomain.CurrentDomain.GetSolutionAssemblies(new string[] { "Normandy.Identity" }).ToArray();
            services.AutoDI(assemblys);

            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services, NormandyIdentityOptions config)
        {
            services.AddScoped<LogMessageHttpHandler>();

            services
                .AddHttpClient(AuthType.AuthCenter.ToString())
                .AddHttpMessageHandler<LogMessageHttpHandler>();

            return services;
        }
    }
}
