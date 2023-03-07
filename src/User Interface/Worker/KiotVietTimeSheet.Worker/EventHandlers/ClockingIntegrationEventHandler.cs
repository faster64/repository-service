using KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class ClockingIntegrationEventHandler :
        IIntegrationEventHandler<CreateMultipleClockingIntegrationEvent>,
        IIntegrationEventHandler<UpdateMultipleClockingIntegrationEvent>,
        IIntegrationEventHandler<RejectMultipleClockingIntegrationEvent>,
        IIntegrationEventHandler<SwappedClockingIntegrationEvent>,
        IIntegrationEventHandler<ChangedClockingIntegrationEvent>,
        IIntegrationEventHandler<UpdateAutoKeepingIntegrationEvent>
    {
        private readonly ClockingAuditProcess _clockingAuditProcess;

        public ClockingIntegrationEventHandler(ClockingAuditProcess clockingAuditProcess)
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

        public async Task Handle(ChangedClockingIntegrationEvent @event)
        {
            await _clockingAuditProcess.WriteChangeClockingLogAsync(@event);
        }

        public async Task Handle(UpdateAutoKeepingIntegrationEvent @event)
        {
            await _clockingAuditProcess.WriteAutoKeepingLogAsync(@event);
        }
    }
}
