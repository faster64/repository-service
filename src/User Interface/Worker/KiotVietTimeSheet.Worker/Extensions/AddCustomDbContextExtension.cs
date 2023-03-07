using System;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer.Converters;

namespace KiotVietTimeSheet.AuditTrailWorker.Extensions
{
    public static class AddAuditCustomDbContextExtension
    {
        public static void AddCustomDbContext(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionFactory>(c =>
            {
                SqlServer2016Dialect.Provider.GetStringConverter().UseUnicode = true;
                SqlServer2016Dialect.Provider.RegisterConverter<DateTime>(new SqlServerDateTime2Converter());
                OrmLiteConfig.CommandTimeout = 60;
                var dbFactory = new OrmLiteConnectionFactory(GetTimeSheetConnection(c), SqlServer2016Dialect.Provider)
                {
                    AutoDisposeConnection = true,
                };
                
                return dbFactory;
            });

            PocoMapping.Factory();

            services.AddDbContext<EfIntegrationEventLogContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetTimeSheetConnection(sp));
            });

            services.AddDbContext<EfDbContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetTimeSheetConnection(sp))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
            var infrastructureConfiguration = new InfrastructureConfiguration(configuration); //NOSONAR

            var timeSheetConnection = InfrastructureConfiguration.ConnectionString;

            if (eventContextSvc.Context.GroupId == 0) return timeSheetConnection;

            var shardingConnection = configuration.GetConnectionString($"KiotVietTimeSheetDatabaseS{eventContextSvc.Context.GroupId}");

            if (!string.IsNullOrEmpty(shardingConnection)) timeSheetConnection = shardingConnection;

            return timeSheetConnection;
        }
    }
}
