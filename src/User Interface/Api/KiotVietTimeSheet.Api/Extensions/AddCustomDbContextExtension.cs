using System;
using System.Linq;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Infrastructure.Configuration;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.DbMaster.Models;
using KiotVietTimeSheet.Infrastructure.DbRetail;
using KiotVietTimeSheet.Infrastructure.EventBus.EventLog.EntityFrameworkCore;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.Infrastructure.Securities.KvAuth;
using KiotVietTimeSheet.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer.Converters;

namespace KiotVietTimeSheet.Api.Extensions
{
    public static class AddCustomDbContextExtension
    {
        public static void AddCustomDbContext(this IServiceCollection services)
        {
            services.AddDbContext<DbMasterContext>((sp, opts) =>
            {
                opts.UseSqlServer(InfrastructureConfiguration.ConnectionStringDbMaster).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IDbConnectionFactory>(c =>
            {
                OrmLiteConfig.CommandTimeout = 60;
                SqlServer2016Dialect.Provider.GetStringConverter().UseUnicode = true;
                SqlServer2016Dialect.Provider.RegisterConverter<DateTime>(new SqlServerDateTime2Converter());
                var dbFactory = new OrmLiteConnectionFactory(GetTimeSheetConnection(c), SqlServer2016Dialect.Provider)
                {
                    AutoDisposeConnection = true
                };
                return dbFactory;
            });

            services.AddDbContext<EfDbContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetTimeSheetConnection(sp))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDbContext<EfIntegrationEventLogContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetTimeSheetConnection(sp));
            });

            services.AddDbContext<DbRetailContext>((sp, opts) =>
            {
                opts.UseSqlServer(GetRetailConnection(sp));
            });

        }

        /// <summary>
        /// Lấy sharding connectionTimeSheet từ redis cache hoặc DbMaster
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private static string GetTimeSheetConnection(IServiceProvider sp)
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var request = HostContext.GetCurrentRequest();
            var authService = HostContext.ResolveService<AuthenticateService>(request);
            var session = authService.GetSession().ConvertTo<KVSession>();
            var code = session?.CurrentRetailerCode ?? string.Empty;
            if (string.IsNullOrEmpty(code))
            {
                var httpContext = HostContext.Resolve<IHttpContextAccessor>();
                code = Globals.GetRetailerCode(httpContext);
            }
            var cacheClientsManager = sp.GetRequiredService<ICacheClient>();
            var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                code,
                nameof(KiotVietTimeSheet),
                nameof(KvGroup)
            )}*";
            var item = cacheClientsManager.GetOrDefault<KvGroup>(keyObject);
            var timeSheetConnection = InfrastructureConfiguration.ConnectionString;
            if (item == null || string.IsNullOrEmpty(item.TimeSheetConnectionString))
            {
                var masterDbContext = sp.GetRequiredService<DbMasterContext>();
                var retailer = masterDbContext?.KvRetailer.FirstOrDefault(x => x.Code == code);
                if (retailer == null) return timeSheetConnection;

                var kvGroup = masterDbContext?.KvGroup.FirstOrDefault(x => x.Id == retailer.GroupId);
                if (kvGroup == null || string.IsNullOrEmpty(kvGroup.TimeSheetConnectionString))
                    return timeSheetConnection;

                cacheClientsManager.Set(keyObject, kvGroup);
                var shardingConnection = configuration.GetConnectionString(kvGroup.TimeSheetConnectionString);
                if (!string.IsNullOrEmpty(shardingConnection)) timeSheetConnection = shardingConnection;

            }
            else
            {
                var shardingConnection = configuration.GetConnectionString(item.TimeSheetConnectionString);
                if (!string.IsNullOrEmpty(shardingConnection)) timeSheetConnection = shardingConnection;
            }
            return timeSheetConnection;
        }

        private static string GetRetailConnection(IServiceProvider sp)
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var request = HostContext.GetCurrentRequest();
            var authService = HostContext.ResolveService<AuthenticateService>(request);
            var session = authService.GetSession().ConvertTo<KVSession>();
            var code = session?.CurrentRetailerCode ?? string.Empty;
            if (string.IsNullOrEmpty(code))
            {
                var httpContext = HostContext.Resolve<IHttpContextAccessor>();
                code = Globals.GetRetailerCode(httpContext);
            }
            var cacheClientsManager = sp.GetRequiredService<ICacheClient>();
            var keyObject = $@"{CacheKeys.GetEntityCacheKey(
                code,
                nameof(KiotVietTimeSheet),
                nameof(KvGroup)
            )}*";
            var result = configuration.GetConnectionString("KVEntities");
            var item = cacheClientsManager.GetOrDefault<KvGroup>(keyObject);
            if (item == null || string.IsNullOrEmpty(item.ConnectionString))
            {
                var masterDbContext = sp.GetRequiredService<DbMasterContext>();
                var groupId = masterDbContext?.KvRetailer.Where(x => x.Code == code).Select(x => x.GroupId).FirstOrDefault();
                if (groupId == null) return result;

                var kvGroup = masterDbContext?.KvGroup.FirstOrDefault(x => x.Id == groupId);
                if (kvGroup == null || string.IsNullOrEmpty(kvGroup.ConnectionString))
                    return result;

                cacheClientsManager.Set(keyObject, kvGroup);
                var shardingConnection = configuration.GetConnectionString(kvGroup.ConnectionString.Replace("name=", string.Empty));
                if (!string.IsNullOrEmpty(shardingConnection)) result = shardingConnection;
            }
            else
            {
                var shardingConnection = configuration.GetConnectionString(item.ConnectionString.Replace("name=", string.Empty));
                if (!string.IsNullOrEmpty(shardingConnection)) result = shardingConnection;
            }
            return result;
        }
    }
}
