using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.BackgroundTasks.Configuration;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.DbMaster.Models;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.EventBus;
using KiotVietTimeSheet.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using StringExtensions = ServiceStack.StringExtensions;

namespace KiotVietTimeSheet.BackgroundTasks.Services
{
    public class AutoGenerateClockingBackgroundService : BackgroundService
    {
        private readonly IEventBus _eventBus;
        private readonly ICacheClient _cacheClient;
        private readonly IConfiguration _configuration;
        private readonly AutoGenerateClockingConfiguration _autoGenerateClockingConfiguration;
        private readonly int _delayTime;
        private readonly int _progressTime;

        public AutoGenerateClockingBackgroundService(IEventBus eventBus, IConfiguration configuration, ICacheClient cacheClient, IServiceProvider serviceProvider)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _cacheClient = cacheClient;

            _autoGenerateClockingConfiguration = serviceProvider.GetRequiredService<IOptions<AutoGenerateClockingConfiguration>>().Value;
            _delayTime = _autoGenerateClockingConfiguration.IntervalMinute * 60 * 1000;
            _progressTime = _delayTime * 3;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var isMain = _configuration.GetSection("IsMain").Get<bool>();
            if (!isMain)
            {
                Log.Information("AutoGenerateClockingBackgroundService off by is not main");
                return;
            }

            if (!_autoGenerateClockingConfiguration.IsEnable || _autoGenerateClockingConfiguration.GroupIds?.Any() != true)
            {
                Log.Information($"AutoGenerateClockingBackgroundService off by is not enable. AutoGenerateClockingConfiguration = {StringExtensions.ToJson(_autoGenerateClockingConfiguration)}");
                return;
            }

            Log.Information($"AutoGenerateClockingBackgroundService start with autoGenerateClockingConfiguration = {StringExtensions.ToJson(_autoGenerateClockingConfiguration)}");

            var dbMasterContextOptions = new DbContextOptionsBuilder<DbMasterContext>()
                .UseSqlServer(_configuration.GetConnectionString("MasterDb"))
                .Options;

            while (true)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        Log.Information("Stopped AutoGenerateClockingBackgroundService");
                        break;
                    }
                    Log.Information($"AutoGenerateClockingBackgroundService running with autoGenerateClockingConfiguration = {StringExtensions.ToJson(_autoGenerateClockingConfiguration)}");

                    using (var dbMasterContext = new DbMasterContext(dbMasterContextOptions))
                    {
                        await dbMasterContext.Database.OpenConnectionAsync(stoppingToken);

                        var groups = await dbMasterContext.KvGroup.Where(x => _autoGenerateClockingConfiguration.GroupIds.Contains(x.Id)).ToListAsync(stoppingToken);
                        if (groups?.Any() != true) continue;

                        foreach (var @group in groups)
                        {
                            await HandleForGroup(@group, dbMasterContext, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("AutoGenerateClockingBackgroundService error", ex);
                }
                finally
                {
                    await Task.Delay(_delayTime, stoppingToken);
                }
            }
        }

        private async Task HandleForGroup(KvGroup @group, DbMasterContext dbMasterContext, CancellationToken stoppingToken)
        {
            var dbTimeSheetContextOptions = new DbContextOptionsBuilder<EfDbContext>();
            var connectionString = _configuration.GetConnectionString(@group.TimeSheetConnectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                Log.Error($"AutoGenerateClockingBackgroundService can not get connection string key: {@group.TimeSheetConnectionString}");
                return;
            }

            dbTimeSheetContextOptions.UseSqlServer(connectionString);

            using (var dbTimeSheetContext = new EfDbContext(dbTimeSheetContextOptions.Options))
            {
                await dbTimeSheetContext.Database.OpenConnectionAsync(stoppingToken);

                var expression = new FindTimeSheetAutoGenerateClockingSpec(_autoGenerateClockingConfiguration.QuantityDayCondition).GetExpression();
                var tenantIdValids = await dbTimeSheetContext.TimeSheets.Where(expression).Select(x => x.TenantId).Distinct().ToListAsync(stoppingToken);

                tenantIdValids = FilterTenant(tenantIdValids);

                if (tenantIdValids?.Any() != true) return;

                var tenants = await dbMasterContext.KvRetailer.Where(x => tenantIdValids.Contains(x.Id)).Select(x => new { x.Id, x.Code, x.GroupId }).ToListAsync(stoppingToken);

                foreach (var tenant in tenants)
                {
                    // Bỏ qua khi trường hợp tách shard data vẫn lưu db timesheet cũ
                    if (tenant.GroupId != @group.Id)
                    {
                        Log.Information($"AutoGenerateClockingBackgroundService tenant invalid (currentGroup = {@group.Id}, tenant.GroupId = {tenant.GroupId}) --> Ignore push message");
                        continue;
                    }

                    var cacheKey = Globals.GetCacheAutoGenerateClockingInprogress(tenant.Id);
                    var cacheData = _cacheClient.GetOrDefault<object>(cacheKey);

                    if (_cacheClient.GetOrDefault<object>(cacheKey) != null)
                    {
                        Log.Information($"AutoGenerateClockingBackgroundService tenant inprogress (TenantId = {tenant.Id}, CacheData = {StringExtensions.ToJson(cacheData)}) --> Ignore push message");
                        continue;
                    }

                    var @event = new CreateAutoGenerateClockingIntegrationEvent
                    {
                        QuantityDayCondition = _autoGenerateClockingConfiguration.QuantityDayCondition,
                        QuantityDayAdd = _autoGenerateClockingConfiguration.QuantityDayAdd,
                        EmployeeWorkingInDay = _autoGenerateClockingConfiguration.EmployeeWorkingInDay,
                        HandleExpiredAt = DateTime.Now.AddMilliseconds(_delayTime - 1000)
                    };
                    var user = new SessionUser { IsAdmin = true };
                    @event.SetContext(new IntegrationEventContext(tenant.Id, 0, 0, @group.Id, tenant.Code, user));

                    _eventBus.Publish(@event);
                    _cacheClient.Set(cacheKey, new { CreatedTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }, TimeSpan.FromMilliseconds(_progressTime));
                }
            }
        }

        private List<int> FilterTenant(List<int> tenantIds)
        {
            if (_autoGenerateClockingConfiguration.ExcludeTenantIds?.Any() == true)
            {
                tenantIds = tenantIds?.Where(x => !_autoGenerateClockingConfiguration.ExcludeTenantIds.Contains(x))?.ToList();
            }
            if (_autoGenerateClockingConfiguration.IncludeTenantIds?.Any() == true)
            {
                tenantIds = tenantIds?.Where(x => _autoGenerateClockingConfiguration.IncludeTenantIds.Contains(x))?.ToList();
            }

            return tenantIds;
        }
    }
}
