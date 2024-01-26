using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Normandy.Infrastructure.Log.Extensions;
using Serilog;
using System;
using System.IO;

namespace Normandy.Infrastructure.Log
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// 从配置文件加载日志
        /// 1.增加自定义日志过滤Filter
        /// 2.添加Elk 格式配置  
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configBuilder"></param>
        /// <param name="configFileName"></param>
        /// <param name="excludeFilterOptionConfigKey"></param>
        /// <returns></returns>
        public static IHostBuilder UseLog(
            this IHostBuilder app, 
            IConfigurationBuilder configBuilder, 
            string configFileName = null,
            string excludeFilterOptionConfigKey = "ExcludeLogFilter")
        {
            if(!string.IsNullOrWhiteSpace(configFileName))
            {
                configBuilder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName), false, true);
            }          

            app.ConfigureServices(services => 
            {
                services.AddHttpContextAccessor();
            });

            app.UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration)
                .AddExcludeFilter(hostContext.Configuration, excludeFilterOptionConfigKey)
                .Enrich.WithElk();
            });

            return app;
        }
    }
}
