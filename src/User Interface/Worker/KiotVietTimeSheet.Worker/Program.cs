using System;
using System.IO;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Service.Impls;
using KiotVietTimeSheet.Application.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using KiotVietTimeSheet.AuditTrailWorker.Extensions;
using KiotVietTimeSheet.AuditTrailWorker.Infras.KiotVietInternalService;
using KiotVietTimeSheet.AuditTrailWorker.Services;
using KiotVietTimeSheet.Infrastructure.EventBus.RabbitMQ;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.Caching.Redis;
using KiotVietTimeSheet.Utilities;
using MediatR;
using ServiceStack;
using Serilog.Exceptions;

namespace KiotVietTimeSheet.AuditTrailWorker
{
    public class Program
    {
        protected Program()
        {
        }

        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static void Main(string[] args)//NOSONAR
        {

            var configuration = GetConfiguration();
            
            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Audit service starting...");

                Globals.SetBuildVersion();

                Log.Information($"Build version: {Globals.BuildVersion}");

                CreateHostBuilder(configuration, args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Audit Service corrupted", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<RabbitMqEventBusConfiguration>(configuration.GetSection("EventBus:RabbitMq"));
                    services.Configure<KiotVietInternalServiceConfiguration>(
                        configuration.GetSection("KiotVietInternalService"));
                    services.Configure<RedisConfig>(configuration.GetSection("Caching:Redis"));
                    var licenseServicestack = configuration.GetSection("servicestack:license").Value;
                    Licensing.RegisterLicense(licenseServicestack);

                    services.AddAutoMapper();
                    Application.AutoMapperConfigurations.AutoMapping.RegisterMappings();

                    services.AddHostedService<EventBusConsumerService>();
                    services.AddMediatR(typeof(Program));
                    services.AddRedisCache();
                    services.AddEventHandlers();
                    services.AddMasterDbService(configuration);

                    services.AddQueries();
                    services.AddResponsitories();
                    services.AddProcess();
                    services.AddCustomDbContext();
                    services.AddKiotVietInternalService();
                    services.AddEventBus();
                    services.AddScoped<IPermissionService, PermissionService>();
                    services.AddScoped<IAuthService, AuthService>();
                    services.AddExecutionContext();
                });
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return configuration;
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
