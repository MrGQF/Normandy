using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Normandy.Infrastructure.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigServiceCollectionExtensions
    {
        /// <summary>
        /// 初始化配置文件
        /// 使用IConfiguration 获取配置
        /// 目前只支持json配置文件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="filePath"></param>
        /// <param name="optional"></param>
        /// <param name="reloadOnChange"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">文件地址没填写</exception>
        /// <exception cref="NotSupportedException">文件格式非法</exception>
        public static IServiceCollection AddConfigs(
            this IServiceCollection services,
            string filePath,
            bool optional,
            bool reloadOnChange)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!filePath.EndsWith(".json"))
            {
                throw new NotSupportedException(nameof(filePath));
            }

            var configuration =
               new ConfigurationBuilder()
               .AddJsonFile(filePath, optional: optional, reloadOnChange: reloadOnChange)
               .Build();

            services.AddSingleton<IConfiguration>(configuration);
            return services;
        }
    }
}
