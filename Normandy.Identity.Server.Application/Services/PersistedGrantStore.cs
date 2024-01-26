using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Normandy.Identity.Server.Application.Services
{
    /// <summary>
    /// 持久化令牌
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private const string _dateFormatString = "yyyy-MM-dd HH:mm:ss";
        private const string SplitStr = ":";
        private readonly IRedisDatabase redisDatabase;

        public PersistedGrantStore(
            IRedisDatabase redisDatabase)
        {
            this.redisDatabase = redisDatabase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var result = new List<PersistedGrant>();
            await GetOrRemoveAllHandler(
                filter,
                async (db, indexKey, key) =>
                {
                    var item = await redisDatabase.GetAsync<PersistedGrant>(key);
                    if (item == null)
                    {
                        return;
                    }
                    result.Add(item);
                });

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var val = await redisDatabase.GetAsync<PersistedGrant>(key);
            return val;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            await GetOrRemoveAllHandler(
                filter,
                async (db, grant, key) =>
                {
                    IndexCacheHandler(filter, async item =>
                    {
                        await db.SetRemoveAsync(item, key);
                    });

                    await db.KeyDeleteAsync(key);
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            var grant = await GetAsync(key);
            if (grant == null)
            {
                return;
            }

            var filter = new PersistedGrantFilter
            {
                ClientId = grant.ClientId,
                SubjectId = grant.SubjectId,
                SessionId = grant.SessionId,
                Type = grant.Type,
            };

            var db = redisDatabase.Database;
            var indexKey = IndexCacheHandler(filter, async item =>
            {
                await db.SetRemoveAsync(item, $"\"{key}\"");
            });

            await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// 使用Set存储缓存值
        public async Task StoreAsync(PersistedGrant grant)
        {
            if (grant == null)
            {
                throw new ArgumentNullException(nameof(grant));
            }

            var trans = redisDatabase.Database.CreateTransaction();

            var expiry = grant.Expiration.Value.ToLocalTime();

            // 添加值
            await redisDatabase.AddAsync(grant.Key, grant, expiry);

            // 添加索引值
            var filter = new PersistedGrantFilter
            {
                ClientId = grant.ClientId,
                SubjectId = grant.SubjectId,
                SessionId = grant.SessionId,
                Type = grant.Type,
            };

            IndexCacheHandler(
                filter,
                async key =>
                {
                    var exist = await redisDatabase.SetContainsAsync(key, grant.Key);
                    if(!exist)
                    {
                        await redisDatabase.SetAddAsync(key, grant.Key);
                        await redisDatabase.UpdateExpiryAsync(key, expiry);
                    }                   
                });

            await trans.ExecuteAsync();
        }

        private async Task GetOrRemoveAllHandler(
            PersistedGrantFilter filter,
            Action<IDatabase, PersistedGrantFilter, string> operation)
        {
            var db = redisDatabase.Database;

            var indexKey = IndexCacheHandler(
                filter,
                null);
            var keyList = db.SetScan(indexKey);

            foreach (var item in keyList.Distinct())
            {
                operation?.Invoke(db, filter, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="operation"></param>
        /// <returns> 返回索引key </returns>
        private string IndexCacheHandler(
             PersistedGrantFilter filter,
            Action<string> operation)
        {
            var indexKey = filter.SubjectId;
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                operation?.Invoke(indexKey);
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                indexKey += $"{SplitStr}{filter.SessionId}";
                operation?.Invoke(indexKey);
            }

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                indexKey += $"{SplitStr}{filter.ClientId}";
                operation?.Invoke(indexKey);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                indexKey += $"{SplitStr}{filter.Type}";
                operation?.Invoke(indexKey);
            }

            return indexKey;
        }

        private HashEntry[] GetHashEntries(PersistedGrant grant)
        {
            return new[]
            {
                new HashEntry(nameof(grant.Key), grant.Key),
                new HashEntry(nameof(grant.Type), grant.Type),
                new HashEntry(nameof(grant.SubjectId), grant.SubjectId??string.Empty),
                new HashEntry(nameof(grant.SessionId), grant.SessionId),
                new HashEntry(nameof(grant.ClientId), grant.ClientId),
                new HashEntry(nameof(grant.Description), grant.Description),
                new HashEntry(nameof(grant.CreationTime), grant.CreationTime.ToString(_dateFormatString)),
                new HashEntry(nameof(grant.Expiration), grant.Expiration == null ? default(DateTime).ToString(_dateFormatString) : grant.Expiration.Value.ToString(_dateFormatString)),
                new HashEntry(nameof(grant.ConsumedTime), grant.ConsumedTime == null ? default(DateTime).ToString(_dateFormatString) : grant.ConsumedTime.Value.ToString(_dateFormatString)),
                new HashEntry(nameof(grant.Data), grant.Data),
            };
        }

        private PersistedGrant GetPersistedGrant(HashEntry[] entries)
        {
            var grant = new PersistedGrant();
            foreach (var item in entries)
            {
                if (item.Name == nameof(grant.Key))
                {
                    grant.Key = item.Value;
                }
                if (item.Name == nameof(grant.Type))
                {
                    grant.Type = item.Value;
                }
                if (item.Name == nameof(grant.SubjectId))
                {
                    grant.SubjectId = item.Value;
                }
                if (item.Name == nameof(grant.SessionId))
                {
                    grant.SessionId = item.Value;
                }
                if (item.Name == nameof(grant.ClientId))
                {
                    grant.ClientId = item.Value;
                }
                if (item.Name == nameof(grant.Description))
                {
                    grant.Description = item.Value;
                }
                if (item.Name == nameof(grant.CreationTime))
                {
                    grant.CreationTime = DateTime.Parse(item.Value);
                }
                if (item.Name == nameof(grant.Expiration))
                {
                    grant.Expiration = DateTime.Parse(item.Value);
                    if (grant.Expiration.Value == default)
                    {
                        grant.Expiration = null;
                    }
                }
                if (item.Name == nameof(grant.ConsumedTime))
                {
                    grant.ConsumedTime = DateTime.Parse(item.Value);
                    if (grant.Expiration.Value == default)
                    {
                        grant.Expiration = null;
                    }
                }
                if (item.Name == nameof(grant.Data))
                {
                    grant.Data = item.Value;
                }
            }

            return grant;
        }
    }
}
