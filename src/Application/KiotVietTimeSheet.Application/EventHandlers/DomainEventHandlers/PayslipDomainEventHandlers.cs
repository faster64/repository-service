using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.PayslipEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class PayslipDomainEventHandlers :
        IEventHandler<CancelPayslipEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public PayslipDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(CancelPayslipEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new CancelPayslipIntegrationEvent(@notification));
        }
    }
}
