using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class CommissionDomainEventHandlers :
        IEventHandler<CreatedCommissionEvent>,
        IEventHandler<UpdatedCommissionEvent>,
        IEventHandler<DeletedCommissionEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public CommissionDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(CreatedCommissionEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new CreatedCommissionIntegrationEvent(@notification));
        }

        public async Task Handle(UpdatedCommissionEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new UpdatedCommissionIntegrationEvent(@notification));
        }

        public async Task Handle(DeletedCommissionEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedCommissionIntegrationEvent(@notification));
        }
    }
}
