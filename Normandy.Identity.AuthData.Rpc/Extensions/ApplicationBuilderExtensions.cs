using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.AuthData.Application.Services;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Dtos;

namespace Normandy.Identity.AuthData.Rpc.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        // <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
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
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<ClientStoreRpcService>();
                endpoints.MapGrpcService<ResourceStoreRpcService>();
                endpoints.MapGrpcService<HealthCheckService>();
            });          

            return app;
        }
    }
}
