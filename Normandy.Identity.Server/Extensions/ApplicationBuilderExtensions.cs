using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Dtos;
using Normandy.Identity.Server.Middleware;
using Serilog;

namespace Normandy.Identity.Server.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IApplicationBuilder Configure(
            this IApplicationBuilder app, 
            IConfiguration configuration, 
            IWebHostBuilder webHostBuilder)
        {
            var config = new NormandyIdentityOptions();
            configuration.Bind(config);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSerilogRequestLogging();
            app.UseMiddleware<RiskMiddleware>();

            app.UseIdentityServer();

            app.UseCors(Consts.MyAllowSpecificOrigins);

            app.UseHealthChecks(Consts.HealthCheckRoute);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            webHostBuilder.UseUrls(config.IdentityServerUrls);

            return app;
        }     
    }
}
