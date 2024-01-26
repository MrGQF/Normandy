using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Dtos;
using Serilog;

namespace Normandy.Identity.UserInfo.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="webHostBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder ConfigureApplicationBuilder(
            this IApplicationBuilder app,
            IConfiguration configuration,
            IWebHostBuilder webHostBuilder)
        {
            var config = new NormandyIdentityOptions();
            configuration.Bind(config);

            app.UseRouting();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(Consts.MyAllowSpecificOrigins);

            app.UseHealthChecks(Consts.HealthCheckRoute);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            webHostBuilder.UseUrls(config.UserInfoWebApiServerUrls);
            return app;
        }
    }
}
