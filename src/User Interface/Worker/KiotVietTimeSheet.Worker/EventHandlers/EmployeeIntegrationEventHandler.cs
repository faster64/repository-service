using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class EmployeeIntegrationEventHandler :
        IIntegrationEventHandler<CreatedEmployeeIntegrationEvent>,
        IIntegrationEventHandler<UpdatedEmployeeIntegrationEvent>,
        IIntegrationEventHandler<DeletedEmployeeIntegrationEvent>,
        IIntegrationEventHandler<DeletedMultipleEmployeeIntegrationEvent>,
        IIntegrationEventHandler<UpdateEmployeeDeviceIntegrationEvent>
    {
        private readonly EmployeeAuditProcess _employeeAuditProcess;
        private readonly ILogger<EmployeeIntegrationEventHandler> _logger;

        public EmployeeIntegrationEventHandler(EmployeeAuditProcess employeeAuditProcess, ILogger<EmployeeIntegrationEventHandler> logger)
        {
            _employeeAuditProcess = employeeAuditProcess;
            _logger = logger;
        }

        public async Task Handle(CreatedEmployeeIntegrationEvent @event)
        {
            _logger.LogDebug($"Handle event {nameof(EmployeeIntegrationEventHandler)}");
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

        public async Task Handle(UpdateEmployeeDeviceIntegrationEvent @event)
        {
            await _employeeAuditProcess.WriteUpdateEmployeeDeviceLogAsync(@event);
        }
    }
}