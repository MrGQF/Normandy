using AutoMapper;
using Google.Protobuf.Collections;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Normandy.Identity.AuthDataRpc;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Enums;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Normandy.Identity.Sever.Application.Services
{
    public class ResourceStore : IResourceStore
    {
        private readonly ResourceStoreRpc.ResourceStoreRpcClient client;
        private readonly IMapper mapper;
        private readonly IRedisDatabase redisDataBase;
        private readonly IConfiguration configuration;
        private readonly ILogger<ResourceStore> logger;

        public ResourceStore(
            ResourceStoreRpc.ResourceStoreRpcClient client,
            IMapper mapper,
            IRedisDatabase redisDataBase,
            IConfiguration configuration,
            ILogger<ResourceStore> logger)
        {
            this.client = client;
            this.mapper = mapper;
            this.redisDataBase = redisDataBase;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null
                || apiResourceNames.Count() <= 0)
            {
                throw new ArgumentNullException(nameof(FindApiResourcesByNameAsync));
            }

            var cacheKey = GetCacheKey(nameof(FindApiResourcesByNameAsync), apiResourceNames);
            var cache = await GetCache<IEnumerable<IdentityServer4.Models.ApiResource>>(cacheKey);
            if (cache != null)
            {
                return cache;
            }

            var request = new ApiResourceRequest();
            request.ApiResourceNames.AddRange(apiResourceNames);
            var result = await client.FindApiResourcesAsyncAsync(request);
            if (result == null
                || result.Code == (int)NormandyIdentityErrorCodes.ApiResourcesByNameNotFound
                || result.Data == null)
            {
                return new List<IdentityServer4.Models.ApiResource>();
            }
            var data = mapper.Map<RepeatedField<ApiResource>, IEnumerable<IdentityServer4.Models.ApiResource>>(result.Data);

            await SetCache(cacheKey, data);
            return data;
        }

        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null
                || scopeNames.Count() <= 0)
            {
                throw new ArgumentNullException(nameof(FindApiResourcesByScopeNameAsync));
            }

            var cacheKey = GetCacheKey(nameof(FindApiResourcesByScopeNameAsync), scopeNames);
            var cache = await GetCache<IEnumerable<IdentityServer4.Models.ApiResource>>(cacheKey);
            if (cache != null)
            {
                return cache;
            }

            var request = new ApiResourceRequest();
            request.ScopeNames.AddRange(scopeNames);
            var result = await client.FindApiResourcesAsyncAsync(request);
            if (result == null
                || result.Code == (int)NormandyIdentityErrorCodes.ApiResourcesByScopeNameNotFound
                || result.Data == null)
            {
                return new List<IdentityServer4.Models.ApiResource>();
            }
            var data = mapper.Map<RepeatedField<ApiResource>, IEnumerable<IdentityServer4.Models.ApiResource>>(result.Data);

            await SetCache(cacheKey, data);
            return data;
        }

        public async Task<IEnumerable<IdentityServer4.Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null
                || scopeNames.Count() <= 0)
            {
                throw new ArgumentNullException(nameof(FindApiScopesByNameAsync));
            }

            var cacheKey = GetCacheKey(nameof(FindApiScopesByNameAsync), scopeNames);
            var cache = await GetCache<IEnumerable<IdentityServer4.Models.ApiScope>>(cacheKey);
            if (cache != null)
            {
                return cache;
            }

            var request = new ApiScopeRequest();
            request.ScopeNames.AddRange(scopeNames);
            var result = await client.FindApiScopesByNameAsyncAsync(request);
            if (result == null
                || result.Code == (int)NormandyIdentityErrorCodes.ApiScopesByNameNotFound
                || result.Data == null)
            {
                return new List<IdentityServer4.Models.ApiScope>();
            }
            var data = mapper.Map<RepeatedField<ApiScope>, IEnumerable<IdentityServer4.Models.ApiScope>>(result.Data);

            await SetCache(cacheKey, data);
            return data;
        }

        public async Task<IEnumerable<IdentityServer4.Models.IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null
                || scopeNames.Count() <= 0)
            {
                throw new ArgumentNullException(nameof(FindIdentityResourcesByScopeNameAsync));
            }

            var cacheKey = GetCacheKey(nameof(FindIdentityResourcesByScopeNameAsync), scopeNames);
            var cache = await GetCache<IEnumerable<IdentityServer4.Models.IdentityResource>>(cacheKey);
            if (cache != null)
            {
                return cache;
            }

            var request = new IdentityResourceRequest();
            request.ScopeNames.AddRange(scopeNames);
            var result = await client.FindIdentityResourcesByScopeNameAsyncAsync(request);
            if (result == null
                || result.Code == (int)NormandyIdentityErrorCodes.IdentityResourcesByScopeNameNotFound
                || result.Data == null)
            {
                return new List<IdentityServer4.Models.IdentityResource>();
            }
            var data = mapper.Map<RepeatedField<IdentityResource>, IEnumerable<IdentityServer4.Models.IdentityResource>>(result.Data);

            await SetCache(cacheKey, data);
            return data;
        }

        public async Task<IdentityServer4.Models.Resources> GetAllResourcesAsync()
        {
            var cacheKey = GetCacheKey(nameof(GetAllResourcesAsync), new List<string>());
            var cache = await GetCache<IdentityServer4.Models.Resources>(cacheKey);
            if (cache != null)
            {
                return cache;
            }

            var request = new ResourcesRequest();
            var result = await client.GetAllResourcesAsyncAsync(request);
            if (result == null
                || result.Code == (int)NormandyIdentityErrorCodes.AllResourcesNotFound
                || result.Data == null
                || result.Data.Count <= 0)
            {
                return new IdentityServer4.Models.Resources();
            }
            var data = mapper.Map<Resources, IdentityServer4.Models.Resources>(result.Data.First());

            await SetCache(cacheKey, data);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<T> GetCache<T>(string key)
        {
            try
            {
                return await redisDataBase.GetAsync<T>(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, default, default);
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        private async Task SetCache<T>(string key, T val)
        {
            try
            {
                var expireSeconds = configuration.GetValue<int>(ConfigKeys.CacheExpireSeconds);
                var expireTimeSpan = TimeSpan.FromSeconds(expireSeconds);
                await redisDataBase.AddAsync(key, val, expireTimeSpan);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, default, default);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private string GetCacheKey(string methodName, IEnumerable<string> names)
        {
            var scopesBytes = JsonSerializer.SerializeToUtf8Bytes(names);
            var scopeBase64 = Convert.ToBase64String(scopesBytes);
            return $"{methodName}-{scopeBase64}";
        }
    }
}
