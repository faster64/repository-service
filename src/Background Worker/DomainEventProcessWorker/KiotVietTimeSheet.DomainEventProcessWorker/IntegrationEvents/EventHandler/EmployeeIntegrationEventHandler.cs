using System.Threading.Tasks;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class EmployeeIntegrationEventHandler :
        IIntegrationEventHandler<CreatedEmployeeIntegrationEvent>,
        IIntegrationEventHandler<UpdatedEmployeeIntegrationEvent>,
        IIntegrationEventHandler<DeletedEmployeeIntegrationEvent>,
        IIntegrationEventHandler<DeletedMultipleEmployeeIntegrationEvent>
    {
        private readonly EmployeeAuditProcess _employeeAuditProcess;

        public EmployeeIntegrationEventHandler(EmployeeAuditProcess employeeAuditProcess)
        {
            _employeeAuditProcess = employeeAuditProcess;

        }

        public async Task Handle(CreatedEmployeeIntegrationEvent @event)
        {
            await _employeeAuditProcess.WriteCreateEmployeeLogAsync(@event);
        }

        public async Task Handle(UpdatedEmployeeIntegrationEvent @event)
        {
            await _employeeAuditProcess.WriteUpdateEmployeeLogAsync(@event);
        }

        public async Task Handle(DeletedEmployeeIntegrationEvent @event)
        {
            await _employeeAuditProcess.WriteDeleteEmployeeLogAsync(@event);
        }

        public async Task Handle(DeletedMultipleEmployeeIntegrationEvent @event)
        {
            await _employeeAuditProcess.WriteDeleteMultipleEmployeeLogAsync(@event);
        }
    }
}
