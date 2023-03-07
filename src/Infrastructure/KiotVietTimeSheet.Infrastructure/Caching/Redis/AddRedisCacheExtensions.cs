using KiotVietTimeSheet.Application.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;

namespace KiotVietTimeSheet.Infrastructure.Caching.Redis
{
    public static class AddRedisCacheExtensions
    {
        public static void AddRedisCache(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var config = sp.GetRequiredService<IOptions<RedisConfig>>().Value;
                var returnRedisManager = RedisClientsManager.GetClientsManager(config);
                return returnRedisManager;
            });

            services.AddSingleton<ICacheClient>(sp => new RedisClientCache(sp.GetRequiredService<IRedisClientsManager>(),
                sp.GetRequiredService<ILogger<RedisClientCache>>()));
        }
    }
}
