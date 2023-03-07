using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.EventBus.Events.AutoKeepingEvents;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;

namespace KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types
{
    public class AutoKeepingProcess : BaseBackgroundProcess
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        private readonly IAutoKeepingWriteOnlyRepository _autoKeepingWriteOnlyRepository;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        private readonly ILogger<AutoKeepingProcess> _logger;
        private readonly IEventBus _eventBus;
        public AutoKeepingProcess(
            IAuthService authService,
            EfDbContext efDbContext,
            IKiotVietInternalService kiotVietInternalService,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            IAutoKeepingWriteOnlyRepository autoKeepingWriteOnlyRepository,
            ILogger<AutoKeepingProcess> logger,
            IEventBus eventBus
            )
            : base(kiotVietInternalService, authService)
        {        
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
            _autoKeepingWriteOnlyRepository = autoKeepingWriteOnlyRepository;
            _integrationEventLogService = integrationEventLogServiceFactory(efDbContext.Database.GetDbConnection());
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task AutoDailyKeepingAsync(AutoKeepingIntegrationEvent @event)
        {
            var autoTimekeepingUid = Guid.NewGuid();
            try 
            {
                List<Clocking> resultClocking = await _autoKeepingWriteOnlyRepository.AutoKeepingAsync(@event.Context.TenantId, @event.StartTime, @event.EndTime, autoTimekeepingUid , @event.JobId);
                //handle result 
                if (resultClocking != null && resultClocking.Any()) {
                    _timeSheetIntegrationEventService.PublishEventWithContext(new UpdateAutoKeepingIntegrationEvent(resultClocking));
                }
            }
            catch (Exception epx) 
            {               
                _logger.LogError($"[AutoKeepingProcess] - has error JobId {@event.JobId} -  tenantId : {@event.Context.TenantId} startTime : {@event.StartTime} endTime : {@event.EndTime} autoTimekeepingUid {autoTimekeepingUid.ToString("N")}, Message : {epx.Message}", epx);
            }            
        }
    }
}