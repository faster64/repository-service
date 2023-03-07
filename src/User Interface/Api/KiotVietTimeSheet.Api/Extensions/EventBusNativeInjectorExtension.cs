using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using KiotVietTimeSheet.Api.Configuration;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.Infrastructure.EventBus;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ;
using KiotVietTimeSheet.Infrastructure.EventBus.Services;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.Extensions.ObjectPool;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class EventBusNativeInjectorExtension
    {
        public static void AddEventBus(this IServiceCollection services)
        {
            var eventTypes = Assembly.GetAssembly(typeof(CreatedShiftIntegrationEvent))
                .GetTypes()
                .Where(t => t.BaseType != null && t.BaseType == typeof(IntegrationEvent))
                .ToList();
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => c => new EfIntegrationEventLogService(c, eventTypes));
            services.AddTransient<ITimeSheetIntegrationEventService, TimeSheetIntegrationEventService>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddSingleton<IPooledObjectPolicy<IModel>>((sp) =>
            {
                var rabbitMqConfiguration = sp.GetService<IApiConfiguration>().EventBusConfiguration;
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitMqConfiguration.Connection,
                    Port = rabbitMqConfiguration.Port,
                    RequestedHeartbeat = rabbitMqConfiguration.RequestedHeartbeat ?? 10,
                    AutomaticRecoveryEnabled = rabbitMqConfiguration.AutomaticRecoveryEnabled,
                    Ssl = {
                    ServerName = rabbitMqConfiguration.Connection,
                    Enabled = rabbitMqConfiguration.UseSsL,
                    Version = System.Security.Authentication.SslProtocols.Tls12
                }
                };

                if (!string.IsNullOrEmpty(rabbitMqConfiguration.VirtualHost))
                {
                    factory.VirtualHost = rabbitMqConfiguration.VirtualHost;
                }

                if (!string.IsNullOrEmpty(rabbitMqConfiguration.UserName))
                {
                    factory.UserName = rabbitMqConfiguration.UserName;
                }

                if (!string.IsNullOrEmpty(rabbitMqConfiguration.Password))
                {
                    factory.Password = rabbitMqConfiguration.Password;
                }

                return new RabbitModelPooledObjectPolicy(factory);
            });

            services.AddSingleton<IEventBus, RabbitMqProducer>();
        }
    }
}
