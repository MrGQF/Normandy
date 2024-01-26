using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Normandy.Identity.Client.Authentication.Application;
using Normandy.Identity.Client.Authorization.Application;
using Normandy.Identity.Client.Configs;
using Normandy.Identity.Client.Domain.Shared;
using Normandy.Identity.Client.Domain.Shared.Consts;
using Normandy.Identity.Client.Domain.Shared.Exceptions;
using Normandy.Infrastructure.Cache;
using Normandy.Infrastructure.HttpClient;
using Normandy.Infrastructure.JobSchedule;
using Normandy.Infrastructure.Util.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Normandy.Identity.Client
{
    public class ClientManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static ClientManager Instance { get => _Instance.Value; }

        /// <summary>
        /// 懒加载创建实例
        /// </summary>
        private static readonly Lazy<ClientManager> _Instance = new Lazy<ClientManager>();

        /// <summary>
        /// 校验参数
        /// </summary>
        /// <param name="httpCommonOptions"></param>
        /// <param name="options"></param>
        private ConfigOptions Validate(HttpCommonOptions httpCommonOptions, ConfigOptions options)
        {
            if (httpCommonOptions == null
               || string.IsNullOrEmpty(httpCommonOptions.AppKey)
               || string.IsNullOrEmpty(httpCommonOptions.AppSecret)
               || string.IsNullOrEmpty(httpCommonOptions.AppKey)
               || string.IsNullOrEmpty(httpCommonOptions.Version))
            {
                throw new ArgumentNullException(nameof(HttpCommonOptions));
            }

            if (string.IsNullOrWhiteSpace(options?.Path))
            {
                options = new ConfigOptions
                {
                    Path = Path.Combine(Directory.GetCurrentDirectory(), ConstKeys.ConfigDefaultFileName),
                };
            }
            else
            {
                options = new ConfigOptions
                {
                    Path = Path.Combine(options.Path, ConstKeys.ConfigDefaultFileName),
                };
            }

            return options;
        }

        /// <summary>
        /// 获取请求配置
        /// </summary>
        /// <param name="httpClientHandler"></param>
        /// <param name="httpCommonOptions"></param>
        /// <returns></returns>
        private IList<HttpClientOptions> GetHttpClientOptions(HttpClientHandler httpClientHandler, HttpCommonOptions httpCommonOptions)
        {
            httpCommonOptions.SDKVersion = ConstKeys.SDKVersion;
            httpCommonOptions.SignMethod = ConstKeys.SignMethod;

            return new List<HttpClientOptions>
            {
                new HttpClientOptions
                {
                    Key = ConstKeys.HttpClientKey,
                    Handler = httpClientHandler,
                    Headers = ClassExtensions.ToDictionary(httpCommonOptions, new List<string> { nameof(HttpCommonOptions.AppSecret) })
                }
            };
        }

        /// <summary>
        /// 启动时初始化        
        /// </summary>
        /// <param name="httpCommonOptions">必填</param>
        /// <param name="configOptions">配置参数,可空</param>
        /// <param name="httpClientHandler">http设置,可空</param>
        /// <exception cref="ConfigInitFailedException">配置文件初始化失败,重启有会恢复默认设置</exception>
        /// <exception cref="ArgumentNullException">参数异常</exception>
        /// 初始化顺序：
        /// 1.配置文件;
        /// 2.缓存;
        /// 3.HttpClientFactory;
        /// 4.认证实例;
        /// 5.授权实例;
        /// 6.定时任务
        public void Init(
            HttpCommonOptions httpCommonOptions,
            HttpClientHandler httpClientHandler = null,
            ConfigOptions configOptions = null)
        {
            configOptions = Validate(httpCommonOptions, configOptions);

            var httpClientOptions = GetHttpClientOptions(httpClientHandler, httpCommonOptions);
            var healthCheckJobOptions = new List<JobConfigs>
            {
                ConfigManager.GetHealthCheckDefaultConfigs()
            };
    
            ServiceFactory.Services
                .AddClientConfigs(configOptions?.Path)
                .AddMemoryCaches()
                .AddHttpClients(httpClientOptions)
                .AddAuthorizations()
                .AddAuthentications()
                .AddJobSchedules(healthCheckJobOptions)
                .SetServiceProvider();

            AddOptionsCache(configOptions, httpCommonOptions);
        }

        /// <summary>
        /// 初始化后，获取认证授权实例
        /// </summary>
        /// <typeparam name="T">目前支持：1.IAuthorization  授权; 2.IAuthentication 认证</typeparam>
        /// <returns></returns>
        /// <exception cref="NotInitException">尚未初始化</exception>
        /// <exception cref="NotSupportedException">范型不支持</exception>
        /// <exception cref="NotImplementedException">认证类型尚未实现</exception>
        public async Task<T> GetAuthAsync<T>()
        {
            T service;

            // GetAuthInstance
            var type = await GetAuthTypeAsync();

            // GetService
            var serviceAccessor = ServiceFactory.Provider?.GetService(typeof(Func<AuthType, T>)) as Func<AuthType, T>;
            if (serviceAccessor == null)
            {
                throw new NotSupportedException();
            }

            service = serviceAccessor(type);
            if (service == null)
            {
                throw new NotImplementedException();
            }

            return service;
        }

        /// <summary>
        /// 获取认证实例类型,默认安全V2 
        /// </summary>
        /// <returns></returns>
        /// 缓存获取，通过健康检查任务切换
        private Task<AuthType> GetAuthTypeAsync()
        {
            var cache = ServiceFactory.Provider.GetService(typeof(IMemoryCache)) as IMemoryCache;
            if (cache == null)
            {
                throw new NotInitException();
            }

            return cache.GetOrCreateAsync(CacheKeys.AuthType, t =>
            {
                return Task.FromResult(AuthType.Security);
            });
        }

        /// <summary>
        /// 添加通用设置数据
        /// </summary>
        /// <param name="httpCommonOptions"></param>
        /// <param name="configOptions"
        private void AddOptionsCache(ConfigOptions configOptions, HttpCommonOptions httpCommonOptions = null)
        {
            // add sign
            var cache = ServiceFactory.Provider.GetRequiredService(typeof(IMemoryCache)) as IMemoryCache;

            if (httpCommonOptions == null)
            {
                return;
            }

            var value = ClassExtensions.ToDictionary(httpCommonOptions);
            cache.Set(CacheKeys.HttpCommonOptions, value);

            // add appsecret
            cache.Set(CacheKeys.AppsecretKey, httpCommonOptions.AppSecret);

            // add configFilePath
            cache.Set(CacheKeys.ConfigFilePathKey, configOptions.Path);
        }
    }
}
