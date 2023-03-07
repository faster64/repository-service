using KiotVietTimeSheet.Application.EventBus.Events.ShiftEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class ShiftIntegrationEventHandler :
        IIntegrationEventHandler<CreatedShiftIntegrationEvent>,
        IIntegrationEventHandler<UpdatedShiftIntegrationEvent>,
        IIntegrationEventHandler<DeletedShiftIntegrationEvent>
    {
        private readonly ShiftAuditProcess _shiftAuditProcess;

        public ShiftIntegrationEventHandler(ShiftAuditProcess shiftAuditProcess)
        {
            _shiftAuditProcess = shiftAuditProcess;
        }

        public async Task Handle(CreatedShiftIntegrationEvent @event)
        {
            await _shiftAuditProcess.WriteCreateShiftLogAsync(@event);
        }

        public async Task Handle(UpdatedShiftIntegrationEvent @event)
        {
            await _shiftAuditProcess.WriteUpdateShiftLogAsync(@event);
        }

        public async Task Handle(DeletedShiftIntegrationEvent @event)
        {
            await _shiftAuditProcess.WriteDeleteShiftLogAsync(@event);
        }
    }
}
