using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class CachingInjectorExtension
    {
        public static void AddCaching(this IServiceCollection services)
        {
            var redisCache = RedisClientsManager.GetClientsManager(InfrastructureConfiguration.RedisConfiguration);
            services.AddSingleton(sc => redisCache);
            services.AddSingleton<Application.Caching.ICacheClient>(sp => new RedisClientCache(sp.GetRequiredService<IRedisClientsManager>(), sp.GetRequiredService<ILogger<RedisClientCache>>()));
        }
    }
}
