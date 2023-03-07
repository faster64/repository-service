using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Messaging;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddRedisMqExtension
    {
        public static void AddRedisMq(this IServiceCollection services)
        {
            var clientsManager = RedisClientsManager.GetClientsManager(InfrastructureConfiguration.RedisMqConfiguration);
            services.AddSingleton<IMessageFactory>(c => new RedisMessageFactory(clientsManager));
        }
    }
}
