using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class ClockingIntegrationEventHandler :
        IIntegrationEventHandler<CreateMultipleClockingIntegrationEvent>,
        IIntegrationEventHandler<UpdateMultipleClockingIntegrationEvent>,
        IIntegrationEventHandler<RejectMultipleClockingIntegrationEvent>,
        IIntegrationEventHandler<SwappedClockingIntegrationEvent>

    {
        private readonly ClockingAuditProcess _clockingAuditProcess;


        public ClockingIntegrationEventHandler(ClockingAuditProcess clockingAuditProcess
            )
        {
            _clockingAuditProcess = clockingAuditProcess;
        }

        public async Task Handle(CreateMultipleClockingIntegrationEvent @event)
        {
            await _clockingAuditProcess.WriteCreateClockingMultipleLogAsync(@event);

        }

        public async Task Handle(RejectMultipleClockingIntegrationEvent @event)
        {

            await _clockingAuditProcess.WriteRejectClockingMultipleLogAsync(@event);
        }

        public async Task Handle(UpdateMultipleClockingIntegrationEvent @event)
        {
            await _clockingAuditProcess.WriteUpdateClockingMultipleLogAsync(@event);

        }

        public async Task Handle(SwappedClockingIntegrationEvent @event)
        {
            await _clockingAuditProcess.WriteSwapClockingLogAsync(@event);
        }
    }
}
