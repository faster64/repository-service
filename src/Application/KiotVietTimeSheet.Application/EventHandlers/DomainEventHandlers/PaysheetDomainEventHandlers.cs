using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.PaysheetEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class PaysheetDomainEventHandlers :
        IEventHandler<CancelPaysheetEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public PaysheetDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(CancelPaysheetEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new CancelPaysheetIntegrationEvent(@notification));
        }
    }
}
