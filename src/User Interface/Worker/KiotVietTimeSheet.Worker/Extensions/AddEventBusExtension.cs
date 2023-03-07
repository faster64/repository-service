﻿using KiotVietTimeSheet.Infrastructure.EventBus;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.IoC.NativeInjector;
using KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class AddAuditEventBusExtension
    {
        public static void AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddScoped<IEventContextService, EventContextService>();
            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();
                var rabbitMqConfiguration = sp.GetService<Microsoft.Extensions.Options.IOptions<RabbitMqEventBusConfiguration>>().Value;
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMqConfiguration.Connection,
                    Port = rabbitMqConfiguration.Port,
                    RequestedHeartbeat = rabbitMqConfiguration.RequestedHeartbeat ?? 10,
                    AutomaticRecoveryEnabled = rabbitMqConfiguration.AutomaticRecoveryEnabled,
                };

                if (!string.IsNullOrEmpty(rabbitMqConfiguration.VirtualHost))
                    factory.VirtualHost = rabbitMqConfiguration.VirtualHost;

                if (!string.IsNullOrEmpty(rabbitMqConfiguration.UserName))
                    factory.UserName = rabbitMqConfiguration.UserName;

                if (!string.IsNullOrEmpty(rabbitMqConfiguration.Password))
                    factory.Password = rabbitMqConfiguration.Password;

                if (rabbitMqConfiguration.UseSsL)
                {
                    factory.Ssl = new SslOption()
                    {
                        ServerName = rabbitMqConfiguration.Connection,
                        Enabled = true,
                        Version = System.Security.Authentication.SslProtocols.Tls12
                    };
                }
                var retryCount = 5;
                if (rabbitMqConfiguration.RetryCount.HasValue)
                    retryCount = rabbitMqConfiguration.RetryCount.Value;
                return new RabbitMqPersistentConnection(factory, logger, retryCount, sp);
            });
            services.AddTransient<IIocContainer>(sp => new IoCContainerNativeInjector(sp));
            services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var rabbitMqConfiguration = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqEventBusConfiguration>>().Value;
                var retryCount = 5;
                if (rabbitMqConfiguration.RetryCount.HasValue)
                {
                    retryCount = rabbitMqConfiguration.RetryCount.Value;
                }

                return new EventBusRabbitMq(
                    rabbitMqPersistentConnection,
                    logger,
                    eventBusSubcriptionsManager,
                    sp,
                    rabbitMqConfiguration.QueueName,
                    retryCount,
                    useScope: true);
            });
        }
    }
}
