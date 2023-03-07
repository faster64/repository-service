
using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Common;
using KiotVietTimeSheet.BackgroundTasks.Configuration;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.BackgroundTasks.Services
{
    //[DisallowConcurrentExecution]
    public class RetryAutoKeepingJob : IJob
    {
        private readonly IEventBus _eventBus;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutoKeepingJob> _logger;
        private readonly AutoTimeKeepingConfiguration _autoTimeKeepingConfiguration;
       
        public RetryAutoKeepingJob()
        { }
        public RetryAutoKeepingJob(IServiceProvider serviceProvider, 
            ILogger<AutoKeepingJob> logger,
            IEventBus eventBus, 
            IConfiguration configuration)
        {
            _logger = logger;
            _eventBus = eventBus;
            _configuration = configuration;
            _autoTimeKeepingConfiguration = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AutoTimeKeepingConfiguration>>().Value;            
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"RetryAutoKeepingJob started at {DateTime.Now}");
            var groupId = 0;
            try
            {
                groupId = Convert.ToInt32(context.MergedJobDataMap.Get("groupId"));
            }
            catch (Exception epx)
            {
                _logger.LogWarning($"[RetryAutoKeepingJob] can not get groupId from context, groupId: {context.MergedJobDataMap.Get("groupId")}",epx);
            }
            
            var orthersConnectionString = _configuration.GetConnectionString($"KiotVietTimeSheetDatabase");
            var connectionString = _configuration.GetConnectionString($"KiotVietTimeSheetS{groupId}Database");
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                _logger.LogInformation($"[RetryAutoKeepingJob] running with group -  {groupId}");                                
            }            
            try
            {
                var dbMasterContextOptions = new DbContextOptionsBuilder<DbMasterContext>()
                                    .UseSqlServer(_configuration.GetConnectionString("MasterDb"))
                                    .Options;

                using (var tokenSource = new CancellationTokenSource(180000))
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
                            _logger.LogError($"[RetryAutoKeepingJob] can not get connection string key with group -  {groupId}");
                            return;
                        }

                        dbTimeSheetContextOptions.UseSqlServer(connectionString);

                        using (var dbTimeSheetContext = new EfDbContext(dbTimeSheetContextOptions.Options))
                        {
                            await dbTimeSheetContext.Database.OpenConnectionAsync(tokenSource.Token);
                            var curentDate = DateTime.Now.Date.AddSeconds(1);
                            var schedulesEventMiss = await dbTimeSheetContext.CronSchedule.Where(x =>
                                                                !x.IsRunning &&
                                                                x.IsActive &&
                                                                x.Type == EventTypeStatic.AutoKeepingJob &&
                                                                x.LastSync < curentDate &&
                                                                x.NextRun < curentDate
                                                            ).ToListAsync();
                                                       

                            var isUpdateDb = false;
                            foreach (var schedule in schedulesEventMiss)
                            {
                                schedule.IsRunning = true;
                                schedule.ModifiedDate = DateTime.Now;
                                var startTime = schedule.NextRun.Date.AddMinutes(-1 - _autoTimeKeepingConfiguration.GetBeforeTimeAmplitude());
                                var endTime = schedule.NextRun.Date.AddDays(1).AddMinutes(-_autoTimeKeepingConfiguration.GetAfterTimeAmplitude());
                                var @event = new AutoKeepingIntegrationEvent
                                {
                                    JobId = schedule.Id,
                                    StartTime = startTime,
                                    EndTime = endTime
                                };
                                @event.SetContext(new IntegrationEventContext(schedule.TenantId, 0, 0, groupId, schedule.TenantCode, new SessionUser { IsAdmin = true, RetailerId = schedule.TenantId, GroupId = groupId }));

                                _eventBus.Publish(@event);
                                dbTimeSheetContext.Update(schedule);
                                isUpdateDb = true;                                
                            }

                            if(isUpdateDb)
                            {
                                await dbTimeSheetContext.SaveChangesAsync();
                            }                               
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RetryAutoKeepingJob] error");
                await Task.Delay(100000);
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
