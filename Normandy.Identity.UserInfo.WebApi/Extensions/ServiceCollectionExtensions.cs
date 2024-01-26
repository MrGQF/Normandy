using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.UserInfo.WebApi.Controllers;
using Normandy.Infrastructure.DI;
using Normandy.Infrastructure.Log.Extensions;
using Normandy.Infrastructure.Mapper;
using Normandy.Infrastructure.Util.Filter;
using Normandy.Infrastructure.Util.Reflection;
using System;

namespace Normandy.Identity.UserInfo.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            var configs = new NormandyIdentityOptions();
            configuration.Bind(configs);

            services.AddGrpcClients(configs);

            services.AddAuthentications(configs);

            services.AddAuthorization();

            services.AddAutoMapper();

            services.AddDI();

            services.AddHealthChecks().AddCheck<CustomHealthCheck>(nameof(CustomHealthCheck));

            services.AddControllers(configure =>
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

            return services;
        }

        /// <summary>
        /// 添加认证中间件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthentications(this IServiceCollection services, NormandyIdentityOptions configs)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = configs.AuthorityAddress;
                options.RequireHttpsMetadata = false;
                options.ApiName = configs.UserInfoApiName;
                options.ApiSecret = configs.UserInfoApiSecret;
                options.EnableCaching = true;
            });

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

            services.AddScoped<LogMessageHttpHandler>();
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
        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            var assemblys = AppDomain.CurrentDomain.GetSolutionAssemblies(new string[] { "Normandy.Identity" });
            services.AutoDI(assemblys);
            return services;
        }
    }
}
