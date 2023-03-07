using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Application.EventHandlers.DomainEventHandlers
{
    public class EmployeeDomainEventHandlers :
        IEventHandler<DeletedEmployeeEvent>
    {
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;
        public EmployeeDomainEventHandlers(ITimeSheetIntegrationEventService timeSheetIntegrationEventService)
        {
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task Handle(DeletedEmployeeEvent @notification, CancellationToken cancellationToken)
        {
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedEmployeeIntegrationEvent(@notification));
        }
    }
}
