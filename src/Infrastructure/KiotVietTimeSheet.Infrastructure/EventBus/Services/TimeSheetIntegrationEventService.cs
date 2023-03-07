using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.EventBus.Models;
using KiotVietTimeSheet.Infrastructure.Persistence.Ef;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.EventBus.Services
{
    public class TimeSheetIntegrationEventService : ITimeSheetIntegrationEventService
    {
        private readonly IAuthService _authService;
        private readonly IEventBus _eventBus;
        private readonly EfDbContext _efDbContext;
        private readonly IIntegrationEventLogService _integrationEventLogService;
        private readonly ILogger<TimeSheetIntegrationEventService> _logger;

        public TimeSheetIntegrationEventService(
            IEventBus eventBus,
            EfDbContext efDbContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            IAuthService authService,
            ILogger<TimeSheetIntegrationEventService> logger
        )
        {
            _eventBus = eventBus;
            _efDbContext = efDbContext;
            _authService = authService;
            _integrationEventLogService = integrationEventLogServiceFactory(_efDbContext.Database.GetDbConnection());
            _logger = logger;
        }

        public TimeSheetIntegrationEventService(
            IEventBus eventBus,
            EfDbContext efDbContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory
        )
        {
            _eventBus = eventBus;
            _efDbContext = efDbContext;
            _integrationEventLogService = integrationEventLogServiceFactory(_efDbContext.Database.GetDbConnection());
        }

        public TimeSheetIntegrationEventService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task PublishEventsToEventBusAsync(Guid transactionId)
        {
            var pendingEventLogs = await _integrationEventLogService.GetPublishableEventAsync(transactionId);
            await PublishEventToEventBusAsync(pendingEventLogs);
        }
        public async Task PublishEventsToEventBusAsync(IntegrationEvent @event)
        {
            await AddEventAsync(@event);
            var pendingEventLogs = await _integrationEventLogService.GetPublishableEventAsync(@event.Id);
            await PublishEventToEventBusAsync(pendingEventLogs);
        }

        public async Task PublishEventsToEventBusByIdAsync(Guid eventID)
        {
            var pendingEventLogs = await _integrationEventLogService.GetPublishableEventByIdAsync(eventID);
            await PublishEventToEventBusAsync(pendingEventLogs);
        }

        public void PublishEvent(IntegrationEvent @event)
        {
            _eventBus.Publish(@event);
        }

        public void PublishEventWithContext(IntegrationEvent @event)
        {
            @event.SetContext(new IntegrationEventContext(
                _authService.Context.TenantId,
                _authService.Context.BranchId,
                _authService.Context.User.Id,
                _authService.Context.User.GroupId,
                _authService.Context.TenantCode,
                _authService.Context.User,
                _authService.Context.Language));
            _eventBus.Publish(@event);
        }

        private async Task PublishEventToEventBusAsync(IEnumerable<IntegrationEventLogEntry> eventLogs)
        {
            foreach (var eventLog in eventLogs)
            {
                try
                {
                    await _integrationEventLogService.MarkEventAsInProgressAsync(eventLog.EventId);
                    _eventBus.Publish(eventLog.IntegrationEvent);
                    await _integrationEventLogService.MarkEventAsPublishedAsync(eventLog.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await _integrationEventLogService.MarkEventAsFailedAsync(eventLog.EventId);
                }
            }
        }

        public async Task RetryPublishFailedEventToEventBusAsync()
        {
            var pendingEventLogs = await _integrationEventLogService.GetFailedEventAsync();
            await PublishEventToEventBusAsync(pendingEventLogs);
        }


        public async Task AddEventAsync(IntegrationEvent @event)
        {
            // set context
            @event.SetContext(new IntegrationEventContext(
                _authService.Context.TenantId,
                _authService.Context.BranchId,
                _authService.Context.User.Id,
                _authService.Context.User.GroupId,
                _authService.Context.TenantCode,
                _authService.Context.User,
                _authService.Context.Language));
            await _integrationEventLogService.SaveEventAsync(@event, _efDbContext.GetCurrentTransaction());
        }

        public async Task AddEventLogAsync(IntegrationEvent eventLog)
        {
            // set context
            if(_authService!=null && _authService.Context!=null)
            {
                eventLog.SetContext(new IntegrationEventContext(
               _authService.Context.TenantId,
               _authService.Context.BranchId,
               _authService.Context.User.Id,
               _authService.Context.User.GroupId,
               _authService.Context.TenantCode,
               _authService.Context.User,
               _authService.Context.Language));
            }                
            
            await _integrationEventLogService.AddEventLogAsync(eventLog, null);
        }

        public async Task AddMultiEventAsync(List<IntegrationEvent> events)
        {
            // set context
            events.ForEach(@event =>
            {
                @event.SetContext(new IntegrationEventContext(
                    _authService.Context.TenantId, 
                    _authService.Context.BranchId, 
                    _authService.Context.User.Id, 
                    _authService.Context.User.GroupId, 
                    _authService.Context.TenantCode, 
                    _authService.Context.User,
                    _authService.Context.Language));
            });
            if (!_authService.Context.IsBackgroundService) await _integrationEventLogService.SaveMultiEventAsync(events, _efDbContext.GetCurrentTransaction());
        }

        
    }
}
