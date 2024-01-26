using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Server.Application.Services;
using Normandy.Identity.Server.Application.Services.Risk;
using Normandy.Identity.Server.Controllers;
using Normandy.Identity.Sever.Application.Services;
using Normandy.Identity.Sever.Application.Services.Risk;
using Normandy.Infrastructure.Cache;
using Normandy.Infrastructure.Cache.Redis;
using Normandy.Infrastructure.DI;
using Normandy.Infrastructure.Log.Extensions;
using Normandy.Infrastructure.Mapper;
using Normandy.Infrastructure.Util.Crypto;
using Normandy.Infrastructure.Util.Filter;
using Normandy.Infrastructure.Util.Reflection;
using RSAExtensions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Yuan.IdentityServer4.Extendsions;

namespace Normandy.Identity.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var configs = new NormandyIdentityOptions();
            configuration.Bind(configs);

            var redisConfiguration = new RedisConfiguration();
            configuration.GetSection(ConfigKeys.IdentityServerRedisOptions).Bind(redisConfiguration);

            services.AddAutoMapper();

            services.ConfigureCache(redisConfiguration, configs.UseHxRedis);

            services.AddGrpcClients(configs);

            services.AddHttpClients(configs);

            services.AddCrypto(configuration);

            services.ConfigureIdentityServer(configs);

            services.AddDI();

            services.AddHealthChecks().AddCheck<CustomHealthCheck>(nameof(CustomHealthCheck));

            services.AddControllersWithViews(configure =>
            {
                configure.Filters.Add<ApiExceptionFilterAttribute>();
                configure.Filters.Add<ApiResponseFilterAttribute>();
                configure.UseCentralRoutePrefix();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(Consts.MyAllowSpecificOrigins,
                    builder => builder.AllowAnyOrigin()
                    .WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS"));
            });

            services.ConfigureNonBreakingSameSiteCookies();

            return services;
        }

        /// <summary>
        /// 添加RPC 调用客户端
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, NormandyIdentityOptions options)
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            services.TryAddScoped<LogMessageHttpHandler>();

            services
                .AddGrpcClient<AuthDataRpc.ClientStoreRpc.ClientStoreRpcClient>(nameof(AuthDataRpc.ClientStoreRpc.ClientStoreRpcClient), o =>
                {
                    o.Address = new Uri(options.AuthDataGrpcAddress);
                })
                .AddHttpMessageHandler<LogMessageHttpHandler>();

            services
                .AddGrpcClient<AuthDataRpc.ResourceStoreRpc.ResourceStoreRpcClient>(nameof(AuthDataRpc.ResourceStoreRpc.ResourceStoreRpcClient), o =>
                {
                    o.Address = new Uri(options.AuthDataGrpcAddress);
                })
                .AddHttpMessageHandler<LogMessageHttpHandler>();

            services
                .AddGrpcClient<UserDataRpc.UserDataRpc.UserDataRpcClient>(nameof(UserDataRpc.UserDataRpc.UserDataRpcClient), o =>
                {
                    o.Address = new Uri(options.UserDataGrpcAddress);
                })
                .AddHttpMessageHandler<LogMessageHttpHandler>();

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCrypto(this IServiceCollection services, IConfiguration configuration)
        {
            var rsa = RSA.Create();
            var privateKey = configuration[ConfigKeys.RsaPrivateKey];
            rsa.ImportPrivateKey(RSAKeyType.Pkcs1, Base64Helper.DecodeToString(privateKey), true);
            services.AddSingleton(rsa);

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisConfigs"></param>
        /// <param name="useHxRedis"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureCache(this IServiceCollection services, RedisConfiguration redisConfigs, bool useHxRedis)
        {
            if (useHxRedis)
            {
                var host = RedisExtensions.GetIpFromEnv();
                var port = RedisExtensions.GetPortFromEnv();
                redisConfigs.Hosts = new RedisHost[1] { new RedisHost { Host = host, Port = Convert.ToInt32(port) } };
            }
            services.AddStackExchangeRedisExtensions<SystemTextJsonSerializer>(redisConfigs);

            return services;
        }

        /// <summary>
        /// 添加认证授权
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, NormandyIdentityOptions options)
        {
            var identityServerBuilder = services.AddIdentityServer(o =>
            {
                o.InputLengthRestrictions = new InputLengthRestrictions
                {
                    Password = options.PasswordLength,
                    TokenHandle = options.TokenHandleLength
                };
            });

            identityServerBuilder
                .AddSigningCredential(new X509Certificate2(Path.Combine(AppContext.BaseDirectory, options.CerName), options.CerPwd));

            identityServerBuilder
                .AddClientStore<ClientStore>()
                .AddResourceStore<ResourceStore>()
                .AddPersistedGrantStore<PersistedGrantStore>()
                .AddProfileService<ProfileService>();

            identityServerBuilder
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                .AddExtensionGrantValidator<SmsGrantValidator>();

            services.AddTransient<ICustomTokenValidator, CustomTokenValidator>();

            services.AddTransient<ResourceOwnerPasswordValidator>();
            return services;
        }

        /// <summary>
        /// 添加Http连接工厂
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClients(this IServiceCollection services, NormandyIdentityOptions config)
        {
            services.TryAddScoped<LogMessageHttpHandler>();

            services
                .AddHttpClient(nameof(NormandyIdentityOptions.RiskCheckUrl))
                .AddHttpMessageHandler<LogMessageHttpHandler>();

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            var assemblys = AppDomain.CurrentDomain.GetSolutionAssemblies(new string[] { "Normandy.Identity" });
            services.AutoDI(assemblys);

            services.TryAddSingleton<PwdLoginRiskProcessor>();
            return services;
        }
    }
}
