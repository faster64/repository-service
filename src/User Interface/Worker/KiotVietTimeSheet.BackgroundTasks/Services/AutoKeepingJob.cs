
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.BackgroundTasks.Configuration;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.Services
{
    [DisallowConcurrentExecution]
    public class AutoKeepingJob : IJob
    {
        private readonly IEventBus _eventBus;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutoKeepingJob> _logger;
        private readonly AutoTimeKeepingConfiguration _autoTimeKeepingConfiguration;
       
        public AutoKeepingJob()
        { }
        public AutoKeepingJob(IServiceProvider serviceProvider, 
            ILogger<AutoKeepingJob> logger,
            IEventBus eventBus, 
            IConfiguration configuration)
        {
            _logger = logger;
            _eventBus = eventBus;
            _configuration = configuration;
            _autoTimeKeepingConfiguration = serviceProvider.GetRequiredService<IOptions<AutoTimeKeepingConfiguration>>().Value;            
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"AutoKeepingJob started at {DateTime.Now}");
            var groupId = 0;
            try
            {
                groupId = Convert.ToInt32(context.MergedJobDataMap.Get("groupId"));
            }
            catch (Exception epx)
            {
                _logger.LogWarning($"AutoKeepingJob can not get groupId from context, groupId: {context.MergedJobDataMap.Get("groupId")}",epx);
            }
            
            var orthersConnectionString = _configuration.GetConnectionString($"KiotVietTimeSheetDatabase");
            var connectionString = _configuration.GetConnectionString($"KiotVietTimeSheetS{groupId}Database");
            var schedules = new List<CronSchedule>();
            if (!string.IsNullOrEmpty(connectionString))
            {
                _logger.LogInformation($"AutoKeepingJob running with group -  {groupId}");                                
            }            
            try
            {
               
               var dbMasterContextOptions = new DbContextOptionsBuilder<DbMasterContext>()
                                    .UseSqlServer(_configuration.GetConnectionString("MasterDb"))
                                    .Options;

                using (var tokenSource = new CancellationTokenSource(480000))
                {
                    using (var dbMasterContext = new DbMasterContext(dbMasterContextOptions))
                    {
                        await dbMasterContext.Database.OpenConnectionAsync(tokenSource.Token);
                        var dbTimeSheetContextOptions = new DbContextOptionsBuilder<EfDbContext>();
                        //try get connection in KvGroup
                        if (string.IsNullOrEmpty(connectionString)) 
                        {
                            var kvGroup = await dbMasterContext.KvGroup.Where(x => x.Id == groupId).SingleAsync();
                            if (kvGroup != null)
                            {
                                connectionString = _configuration.GetConnectionString(kvGroup.TimeSheetConnectionString);                                
                            }
                        }

                        if (string.IsNullOrEmpty(connectionString))
                        {
                            _logger.LogError($"AutoKeepingJob can not get connection string key with group -  {groupId}");
                            return;
                        }

                        dbTimeSheetContextOptions.UseSqlServer(connectionString);

                        using (var dbTimeSheetContext = new EfDbContext(dbTimeSheetContextOptions.Options))
                        {
                            await dbTimeSheetContext.Database.OpenConnectionAsync(tokenSource.Token);
                            var curentDateStart = DateTime.Now.Date;
                            var curentDateEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                            schedules = await dbTimeSheetContext.CronSchedule.Where(x => 
                                                                !x.IsRunning && 
                                                                x.IsActive && 
                                                                x.Type == EventTypeStatic.AutoKeepingJob &&
                                                                x.LastSync < curentDateEnd && 
                                                                (x.NextRun >= curentDateStart  && x.NextRun < curentDateEnd)// check NextRun = curentDateEnd
                                                            ).ToListAsync();

                            var tenantIdValids = schedules.Select(x => x.TenantId).Distinct().ToList();
                            tenantIdValids = FilterTenant(tenantIdValids);

                            if (tenantIdValids?.Any() != true) return;

                            foreach (var schedule in schedules)
                            {
                                schedule.IsRunning = true;
                                schedule.ModifiedDate = DateTime.Now;                                
                                var @event = new AutoKeepingIntegrationEvent
                                {
                                    JobId = schedule.Id,
                                    StartTime = DateTime.Now.Date.AddMinutes(-1 - _autoTimeKeepingConfiguration.GetBeforeTimeAmplitude()),
                                    EndTime = DateTime.Now.Date.AddDays(1).AddMinutes(-_autoTimeKeepingConfiguration.GetAfterTimeAmplitude())
                                };                                
                                @event.SetContext(new IntegrationEventContext(schedule.TenantId, 0, 0, groupId, schedule.TenantCode, new SessionUser { IsAdmin = true, RetailerId = schedule.TenantId, GroupId = groupId }));

                                _eventBus.Publish(@event);

                                dbTimeSheetContext.Update(schedule);
                            }
                            await dbTimeSheetContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AutoKeepingJob] error");
                await Task.Delay(100000);//chờ hệ thống phục hồi
            }
        }
        private List<int> FilterTenant(List<int> tenantIds)
        {
            if (_autoTimeKeepingConfiguration.ExcludeTenantIds?.Any() == true)
            {
                tenantIds = tenantIds?.Where(x => !_autoTimeKeepingConfiguration.ExcludeTenantIds.Contains(x))?.ToList();
            }
            if (_autoTimeKeepingConfiguration.IncludeTenantIds?.Any() == true)
            {
                tenantIds = tenantIds?.Where(x => _autoTimeKeepingConfiguration.IncludeTenantIds.Contains(x))?.ToList();
            }

            return tenantIds;
        }
    }
}
