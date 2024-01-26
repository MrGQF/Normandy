using Microsoft.Extensions.DependencyInjection;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Identity.Client.Domain.Shared.Exceptions;
using Normandy.Infrastructure.Config;
using Normandy.Infrastructure.JobSchedule;
using System;
using System.IO;
using System.Text.Json;

namespace Normandy.Identity.Client.Configs
{
    internal static class ConfigManager
    {
        /// <summary>
        /// 默认SDK 配置
        /// </summary>
        private static ClientOptions DefaultOptions = new ClientOptions
        {
            SecurityDomain = ConstKeys.ConfigDefaultSecurityDoamin,
            SecurityHost = ConstKeys.ConfigDefaultSecurityHost,
            AuthCenterDomain = ConstKeys.ConfigDefaultAuthCenterDomain,
            AuthCenterHost = ConstKeys.ConfigDefaultAuthCenterHost,
            Account = string.Empty,
            Password = string.Empty,
            RiskEventCheckUri = ConstKeys.DefaultRiskEventCheckUri,
            HealthCheck = new HealthCheckConfigs
            {
                Switch = default,
                Id = ConstKeys.JobSchedulerHealthCheckId,
                Corn = ConstKeys.JobSchedulerHealthCheckCron,
                Description = ConstKeys.JobSchedulerHealthCheckDesc,
                StartSecondLimits = ConstKeys.JobSchedulerHealthCheckStartSecondLimits
            },
            HealthStatus = new HealthStatusConfigs
            {
                Status = default,
                Rate = default
            }
        };

        /// <summary>
        /// 添加客户端配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientConfigs(this IServiceCollection services,
            string filePath)
        {            
            if(string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var defaultValue = JsonSerializer.Serialize(DefaultOptions);

            try
            {
                
                JsonFileHelper.GetOrCreateConfigFiles(filePath, defaultValue);
                services.AddConfigs(filePath, false, true).AddProtection();
            }
            catch (Exception ex)
            {
                JsonFileHelper.RebuildConfigFiles(filePath, defaultValue);
                throw new ConfigInitFailedException(null, ex);
            }

            return services;
        }       

        /// <summary>
        /// 获取健康健康默认配置
        /// </summary>
        /// <returns></returns>
        public static JobConfigs GetHealthCheckDefaultConfigs()
        {
            return new JobConfigs
            {
                Switch = DefaultOptions.HealthCheck.Switch,
                Id = DefaultOptions.HealthCheck.Id,
                Cron = DefaultOptions.HealthCheck.Corn,
                Description = DefaultOptions.HealthCheck.Description,
                StartTimeUtc = DateTimeOffset.UtcNow.AddSeconds(DefaultOptions.HealthCheck.StartSecondLimits),
                Type = typeof(HealthCheckJob)
            };
        }
    }
}
