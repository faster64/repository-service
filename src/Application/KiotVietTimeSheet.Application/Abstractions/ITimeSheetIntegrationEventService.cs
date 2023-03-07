using KiotVietTimeSheet.SharedKernel.EventBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Abstractions
{
    public interface ITimeSheetIntegrationEventService
    {
        Task PublishEventsToEventBusByIdAsync(Guid eventID);
        Task PublishEventsToEventBusAsync(Guid transactionId);

        Task PublishEventsToEventBusAsync(IntegrationEvent @event);

        void PublishEventWithContext(IntegrationEvent @event);
        Task RetryPublishFailedEventToEventBusAsync();

        Task AddEventAsync(IntegrationEvent @event);

        Task AddMultiEventAsync(List<IntegrationEvent> events);

        void PublishEvent(IntegrationEvent @event);
        
    }
}