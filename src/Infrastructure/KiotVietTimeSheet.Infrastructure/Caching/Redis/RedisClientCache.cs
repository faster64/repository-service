using KiotVietTimeSheet.Application.Caching;
using ServiceStack.Redis;
using System;
using ServiceStack;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Caching.Redis
{
    public class RedisClientCache : CacheClientBase
    {
        private readonly IRedisClientsManager _redisClientsManager;

        public RedisClientCache(IRedisClientsManager redisClientCacheManager, ILogger<RedisClientCache> logger)
            : base(logger)
        {
            _redisClientsManager = redisClientCacheManager;
        }

        public override void Clear()
        {
            using (var cli = _redisClientsManager.GetClient())
            {
                cli.FlushAll();
            }
        }

        public override T GetOrDefault<T>(string key)
        {
            using (var cli = _redisClientsManager.GetClient())
            {
                return cli.Get<T>(key);
            }
        }

        public override void Remove(string key)
        {
            using (var cli = _redisClientsManager.GetClient())
            {
                cli.Remove(key);
            }
        }

        public override void RemoveByParttern(string parttern)
        {
            using (var cli = _redisClientsManager.GetClient())
            {
                cli.RemoveByPattern(parttern);
            }
        }

        public override void Set<T>(string key, T value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            using (var cli = _redisClientsManager.GetClient())
            {
                cli.Set(key, value, slidingExpireTime ?? TimeSpan.FromMinutes(30));
            }
        }
    }
}
