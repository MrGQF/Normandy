using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Normandy.Identity.AuthDataRpc;
using Normandy.Identity.Domain.Shared.Consts;
using Normandy.Identity.Domain.Shared.Enums;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    public class ClientStore : IClientStore
    {
        private readonly ClientStoreRpc.ClientStoreRpcClient storeClient;
        private readonly IMapper mapper;
        private readonly IRedisDatabase redisDataBase;
        private readonly IConfiguration configuration;
        private readonly ILogger<ClientStore> logger;

        public ClientStore(
            ClientStoreRpc.ClientStoreRpcClient storeClient, 
            IMapper mapper,
            IRedisDatabase redisDataBase,
            IConfiguration configuration,
            ILogger<ClientStore> logger)
        {
            this.storeClient = storeClient;
            this.mapper = mapper;
            this.redisDataBase = redisDataBase;
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            if(string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(FindClientByIdAsync));
            }

            var cacheKey = GetCacheKey(nameof(FindClientByIdAsync), clientId);
            var cache = await GetCacheAsync(cacheKey);          
            if (cache != null)
            {
                return cache;
            }

            var result = await storeClient.FindClientByIdAsyncAsync(new ClientRequest { Id = clientId });
            if (result == null
                || result.Code == (int)NormandyIdentityErrorCodes.ClientByIdNotFound
                || result.Data == null)
            {
                return null;
            }
            var client = mapper.Map<Client>(result.Data);

            await SetCache(cacheKey, client);
            return client; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<Client> GetCacheAsync(string key)
        {
            try
            {
                return await redisDataBase.GetAsync<Client>(key);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, null, null);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        private async Task SetCache(string key, Client val)
        {
            try 
            {
                var expireSeconds = configuration.GetValue<int>(ConfigKeys.CacheExpireSeconds);
                var expireTimeSpan = TimeSpan.FromSeconds(expireSeconds);
                await redisDataBase.AddAsync(key, val, expireTimeSpan);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, null, null);
            }        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private string GetCacheKey(string methodName, string clientId)
        {
            return $"{methodName}-{clientId}";
        }
    }
}
