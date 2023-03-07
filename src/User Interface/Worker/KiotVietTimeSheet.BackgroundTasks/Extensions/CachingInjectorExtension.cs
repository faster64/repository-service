using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class CachingInjectorExtension
    {
        public static void AddCaching(this IServiceCollection services)
        {
            
            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var infrastructureConfiguration = new InfrastructureConfiguration(configuration); //NOSONAR
                var redisCache = RedisClientsManager.GetClientsManager(InfrastructureConfiguration.RedisConfiguration);
                return redisCache;
            });
            services.AddSingleton<Application.Caching.ICacheClient>(sp => new RedisClientCache(sp.GetRequiredService<IRedisClientsManager>(), sp.GetRequiredService<ILogger<RedisClientCache>>()));
        }
    }
}
