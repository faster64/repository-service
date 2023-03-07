using Serilog;
using ServiceStack;
using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker
{
    public class Program
    {
        protected Program(){}
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();

            CreateSerilogLogger(configuration);

            var appHost = new AppHost
            {
                AppSettings = new NetCoreAppSettings(configuration)
            };

            try
            {
                appHost.Container.AddSingleton<IConfiguration>(configuration);

                appHost.Init();
                appHost.Start(configuration.GetValue<string>("WorkerListenerUrl"));
                Log.Information("Service starting...");
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Service corrupted", AppName);
            }
            finally
            {
                Log.CloseAndFlush(); 
                appHost.Dispose();
            }
        }


        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }

        private static void CreateSerilogLogger(IConfiguration configuration)
        {

            Log.Logger = new LoggerConfiguration() //NOSONAR
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
