using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Service.Impls;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.BackgroundTasks.Configuration;
using KiotVietTimeSheet.BackgroundTasks.Extensions;
using KiotVietTimeSheet.BackgroundTasks.Infras.KiotVietInternalService;
using KiotVietTimeSheet.BackgroundTasks.Infras.Mqtt;
using KiotVietTimeSheet.BackgroundTasks.Services;
using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.DomainEvent.Bus.InMemory;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ;
using KiotVietTimeSheet.Infrastructure.EventBus.Services;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using KiotVietTimeSheet.Utilities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Serilog.Exceptions;
using ServiceStack;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace KiotVietTimeSheet.BackgroundTasks
{
    public class Program
    {
        protected Program()
        {}

        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName =
            Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)//NOSONAR
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);
            try
            {
                Log.Information("Background task Service starting...");

                Globals.SetBuildVersion();

                Log.Information($"Build version: {Globals.BuildVersion}");

                CreateHostBuilder(configuration, args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Background Service corrupted", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<RabbitMqEventBusConfiguration>(configuration.GetSection("EventBus:RabbitMq"));
                    services.Configure<KiotVietInternalServiceConfiguration>(configuration.GetSection("KiotVietInternalService"));
                    services.Configure<WorkerConfiguration>(configuration.GetSection("KvCrmIntegrateEndpoint"));
                    services.Configure<WorkerConfiguration>(configuration.GetSection("ConfigMail"));
                    var licenseServicestack = configuration.GetSection("servicestack:license").Value;
                    Licensing.RegisterLicense(licenseServicestack);
                    var options = configuration
                        .Get<WorkerConfiguration>();
                   
                    services.AddSingleton(options);

                    services.Configure<RedisConfig>(configuration.GetSection("Caching:Redis"));
                    
                    services.Configure<AutoGenerateClockingConfiguration>(configuration.GetSection("AutoGenerateClocking"));
                    services.Configure<AutoTimeKeepingConfiguration>(configuration.GetSection("AutoTimeKeeping"));
                    
                    services.AddHostedService<EventBusConsumerService>();
                    services.AddHostedService<AutoGenerateClockingBackgroundService>();
                    services.AddMediatR(typeof(Program));
                    
                    services.AddAutoMapper();
                    Application.AutoMapperConfigurations.AutoMapping.RegisterMappings();
                    var mqttConfig = new MqttClientConfig();
                    configuration.Bind("Mqtt", mqttConfig);

                    services.AddSingleton(sp =>
                    {
                        var logger = sp.GetRequiredService<ILogger<MqttClientWrapper>>();
                        var mqttClientLogger = sp.GetService<MqttClientConfig>();
                        mqttClientLogger.ClientId = $"KiotVietTimeSheet_Mqtt_Bridge_{Guid.NewGuid()}";
                        return new MqttClientWrapper(logger, mqttClientLogger);
                    });

                    services.AddSingleton(mqttConfig);
                    services.AddRedisCache();
                    services.AddEventBus();
                    services.AddEventHandlers();
                    services.AddMasterDbService(configuration);
                    services.AddKiotVietInternalService();
                    services.AddProcess();
                    services.AddCustomDbContext();
                    services.AddQueries();
                    services.AddCommands();
                    services.AddCaching();
                    services.AddWriteOnlyRepositories();
                    services.AddReadOnlyRepositories();
                    services.AddDomainService();
                    services.AddScoped<IEventDispatcher, MediatorEventDispatcher>();
                    services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
                    services.AddScoped<IPermissionService, PermissionService>();
                    services.AddScoped<IAuthService, AuthService>();
                    services.AddExecutionContext();
                    services.AddSingleton<IApplicationConfiguration>(c => new ApplicationConfiguration(configuration));
                    services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(sp => c => new EfIntegrationEventLogService(c, new List<Type>{ typeof(CreateAutoGenerateClockingIntegrationEvent) }));
                    services.AddTransient<ITimeSheetIntegrationEventService, TimeSheetIntegrationEventService>();

                    services.AddQuartzJob(configuration);
                });

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()//NOSONAR
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.WithProperty("AppName", AppName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
