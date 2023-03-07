using ServiceStack.Configuration;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.Caching.Redis
{
    public class RedisClientsManager
    {
        private readonly List<IRedisClientsManager> _pools;

        private static ILogger<RedisClientsManager> _logger;
        private static ILogger<RedisClientsManager> Logger => _logger ?? (_logger = HostContext.Resolve<ILogger<RedisClientsManager>>());

        public RedisClientsManager(IAppSettings settings)
        {
            _pools = new List<IRedisClientsManager>();
            var ls = InfrastructureConfiguration.RedisConfigurations;
            try
            {
                foreach (var config in ls)
                {
                    _pools.Add(GetClientsManager(config));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

        public List<IRedisClientsManager> GetPools()
        {
            return _pools;
        }

        public static IRedisClientsManager GetClientsManager(RedisConfig config)
        {
            try
            {
                if (config == null) return null;
                var maxpoolsize = config.MaxPoolSize > 0 ? config.MaxPoolSize : 40;
                var pooltimeout = config.MaxPoolTimeout > 0 ? config.MaxPoolTimeout : 10;
                if (config.IsSentinel)
                {
                    IEnumerable<string> servers = config.Servers.Split(';');

                    var sentinel = new RedisSentinel(servers, config.SentinelMasterName)
                    {
                        RedisManagerFactory = (master, slaves) => new RedisManagerPool(master, new RedisPoolConfig { MaxPoolSize = maxpoolsize })
                    };
                    if (config.IsStrictPool)
                    {
                        sentinel.RedisManagerFactory = (master, slaves) =>
                            new PooledRedisClientManager(master, master, null, config.DbNumber, maxpoolsize,
                                null);
                    }
                    if (config.WaitBeforeForcingMasterFailover > 0)
                    {
                        sentinel.WaitBeforeForcingMasterFailover = TimeSpan.FromSeconds(config.WaitBeforeForcingMasterFailover);                        
                    }
                    sentinel.OnFailover += x => Logger.LogWarning($"Redis fail over to {sentinel.GetMaster()}");
                    sentinel.HostFilter = host => $"{config.AuthPass}@{host}?db={config.DbNumber}&RetryTimeout=100";
                    var redisClient = sentinel.Start();
                    return redisClient;
                }
                var redisConnection = $"{config.AuthPass}@{config.Servers}";
                return new PooledRedisClientManager(new[] { redisConnection }, new[] { redisConnection }, null, config.DbNumber, maxpoolsize, pooltimeout);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}
