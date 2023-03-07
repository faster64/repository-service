using KiotVietTimeSheet.Application.EventBus.Events.CommissionDetailEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class CommissionDetailIntegrationEventHandler :
        IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationEvent>,
        IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryIntegrationEvent>,
        IIntegrationEventHandler<CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent>,
        IIntegrationEventHandler<DeletedCommissionDetailIntegrationEvent>,
        IIntegrationEventHandler<DeletedCommissionDetailIntegrationAuditEvent>,
        IIntegrationEventHandler<UpdatedValueOfCommissionDetailIntegrationEvent>,
        IIntegrationEventHandler<CreatedCommissionDetailByProductIntegrationAuditEvent>
        
    {
        private readonly CommissionDetailAuditProcess _commissionDetailAuditProcess;

        public CommissionDetailIntegrationEventHandler(CommissionDetailAuditProcess commissionDetailAuditProcess)
        {
            _commissionDetailAuditProcess = commissionDetailAuditProcess;
        }

        public async Task Handle(CreatedCommissionDetailByProductIntegrationEvent @event)
        {

            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductLogAsync(@event);
        }

        public async Task Handle(CreatedCommissionDetailByProductCategoryIntegrationEvent @event)
        {
            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductCategoryLogAsync(@event);
        }

        public async Task Handle(CreatedCommissionDetailByProductCategoryAsyncIntegrationAuditEvent @event)
        {
            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductCategoryAsyncLogAsync(@event);
        }

        public async Task Handle(DeletedCommissionDetailIntegrationEvent @event)
        {
            await _commissionDetailAuditProcess.WriteDeleteCommissionDetailLogAsync(@event);
        }

        public async Task Handle(UpdatedValueOfCommissionDetailIntegrationEvent @event)
        {
            await _commissionDetailAuditProcess.WriteUpdateValueOfCommissionDetailLogAsync(@event);
        }
        public async Task Handle(CreatedCommissionDetailByProductIntegrationAuditEvent @event)
        {
            await _commissionDetailAuditProcess.WriteCreateCommissionDetailByProductAuditLogAsync(@event);
        }
        public async Task Handle(DeletedCommissionDetailIntegrationAuditEvent @event)
        {
            await _commissionDetailAuditProcess.WriteDeleteCommissionDetailLogAsync(@event);
        }
    }
}
