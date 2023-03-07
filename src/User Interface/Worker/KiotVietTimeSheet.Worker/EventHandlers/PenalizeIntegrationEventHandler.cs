

using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class PenalizeIntegrationEventHandler : 
        IIntegrationEventHandler<CreatedPenalizeIntegrationEvent>,
        IIntegrationEventHandler<DeletedPenalizeIntegrationEvent>,
        IIntegrationEventHandler<UpdatedPenalizeIntegrationEvent>
    {
        private readonly PenalizeAuditProcess _penalizeAuditProcess;
        public PenalizeIntegrationEventHandler(PenalizeAuditProcess penalizeAuditProcess)
        {
            _penalizeAuditProcess = penalizeAuditProcess;
        }
        public async Task Handle(CreatedPenalizeIntegrationEvent @event)
        {
            await _penalizeAuditProcess.CreateLogAsync(@event);
        }

        public async Task Handle(UpdatedPenalizeIntegrationEvent @event)
        {
            await _penalizeAuditProcess.UpdateLogAsync(@event);
        }

        public async Task Handle(DeletedPenalizeIntegrationEvent @event)
        {
            await _penalizeAuditProcess.DeleteLogAsync(@event);
        }
    }
}
