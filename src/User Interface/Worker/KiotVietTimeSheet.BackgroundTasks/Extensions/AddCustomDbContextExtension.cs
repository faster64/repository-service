using System;
using System.Text.RegularExpressions;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer.Converters;

namespace KiotVietTimeSheet.BackgroundTasks.Extensions
{
    public static class AddCustomDbContextExtension
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static void AddCustomDbContext(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionFactory>(c =>
            {
                OrmLiteConfig.CommandTimeout = 60;
                SqlServer2016Dialect.Provider.GetStringConverter().UseUnicode = true;
                SqlServer2016Dialect.Provider.RegisterConverter<DateTime>(new SqlServerDateTime2Converter());
                var dbFactory = new OrmLiteConnectionFactory(GetTimeSheetConnection(c), SqlServer2016Dialect.Provider)
                {
                    AutoDisposeConnection = true,
                };
                
                return dbFactory;
            });

            PocoMapping.Factory();

            services.AddDbContext<EfDbContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetTimeSheetConnection(sp))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDbContext<EfIntegrationEventLogContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetTimeSheetConnection(sp));
            });

        }

        /// <summary>
        /// Lấy sharding connectionTimeSheet từ redis cache hoặc DbMaster
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private static string GetTimeSheetConnection(IServiceProvider sp)
        {
            var eventContextSvc = sp.GetRequiredService<IEventContextService>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            Log.Logger = CreateSerilogLogger(configuration);
            var infrastructureConfiguration = new InfrastructureConfiguration(configuration);//NOSONAR
            var timeSheetConnection = InfrastructureConfiguration.ConnectionString;

            var regex = @"(Database=).*?(?=;)";

            var m = Regex.Match(timeSheetConnection, regex);
            var databaseName = m.Success? m.Value : "";

            if (eventContextSvc.Context.GroupId == 0) return timeSheetConnection;

            var shardingConnection = configuration.GetConnectionString($"KiotVietTimeSheetDatabaseS{eventContextSvc.Context.GroupId}");
            m = Regex.Match(shardingConnection, regex);
            databaseName = m.Success ? m.Value : "";

            if (!string.IsNullOrEmpty(shardingConnection)) timeSheetConnection = shardingConnection;
            return timeSheetConnection;
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
        
            return new LoggerConfiguration()//NOSONAR
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.WithProperty("AppName", AppName)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
