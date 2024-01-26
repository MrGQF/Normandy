using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.UserData.Application.Services;

namespace Normandy.Identity.UserData.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder Configure(
            this IApplicationBuilder app,
            IConfiguration configuration)
        {
            var config = new NormandyIdentityOptions();
            configuration.Bind(config);
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<HealthCheckRpcService>();
                endpoints.MapGrpcService<UserInfoRpcService>();
            });

            return app;
        }
    }
}
