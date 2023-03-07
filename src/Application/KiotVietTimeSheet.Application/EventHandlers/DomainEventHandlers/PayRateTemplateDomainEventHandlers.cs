using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class PayRateTemplateDomainEventHandlers :
        IEventHandler<CreatedPayRateTemplateEvent>,
        IEventHandler<UpdatedPayRateTemplateEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public PayRateTemplateDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(CreatedPayRateTemplateEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedPayRateTemplateIntegrationEvent(@notification));
        }

        public async Task Handle(UpdatedPayRateTemplateEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedPayRateTemplateIntegrationEvent(@notification));
        }
    }
}
