using KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class CommissionIntegrationEventHandler :
        IIntegrationEventHandler<CreatedCommissionIntegrationEvent>,
        IIntegrationEventHandler<UpdatedCommissionIntegrationEvent>,
        IIntegrationEventHandler<DeletedCommissionIntegrationEvent>
    {
        private readonly CommissionAuditProcess _commissionAuditProcess;

        public CommissionIntegrationEventHandler(CommissionAuditProcess commissionAuditProcess)
        {
            _commissionAuditProcess = commissionAuditProcess;
        }

        public async Task Handle(CreatedCommissionIntegrationEvent @event)
        {
            await _commissionAuditProcess.WriteCreateCommissionLogAsync(@event);
        }

        public async Task Handle(UpdatedCommissionIntegrationEvent @event)
        {
            await _commissionAuditProcess.WriteUpdateCommissionLogAsync(@event);
        }

        public async Task Handle(DeletedCommissionIntegrationEvent @event)
        {
            await _commissionAuditProcess.WriteDeleteCommissionLogAsync(@event);
        }
    }
}
