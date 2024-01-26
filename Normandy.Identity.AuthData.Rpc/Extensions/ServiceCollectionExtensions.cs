using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Storage;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Infrastructure.Log.Provider;
using Normandy.Infrastructure.Mapper;
using System;
using System.Linq;

namespace Normandy.Identity.AuthData.Rpc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IHostEnvironment environment,
            IConfiguration configuration,
            IWebHostBuilder webHostBuilder)
        {
            var config = new NormandyIdentityOptions();
            configuration.Bind(config);

            services.AddAutoMapper();

            services.AddGrpcHttpApi();
            services.AddGrpc();

            services.AddDbContexts(config);

            services.AddDI();

            webHostBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(config.AuthDataRpcServerHttp2Port, o => o.Protocols =
                    HttpProtocols.Http2);

                options.ListenAnyIP(config.AuthDataRpcServerHttp1Port, o => o.Protocols =
                    HttpProtocols.Http1);
            });

            return services;
        }

        public static IServiceCollection AddDbContexts(this IServiceCollection services, NormandyIdentityOptions config)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new EFCoreLoggerProvider());

            services.AddConfigurationDbContext<ConfigurationDbContext>(options =>
            options.ConfigureDbContext = b => b
            .UseMySql(config.AuthDbConnectionstrings.FirstOrDefault(), new MySqlServerVersion(new Version(5, 7, 20)))
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(loggerFactory));

            return services;
        }

        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            services.TryAddScoped<IClientStore, IdentityServer4.EntityFramework.Stores.ClientStore>();
            services.TryAddScoped<IResourceStore, IdentityServer4.EntityFramework.Stores.ResourceStore>();

            return services;
        }
    }
}
