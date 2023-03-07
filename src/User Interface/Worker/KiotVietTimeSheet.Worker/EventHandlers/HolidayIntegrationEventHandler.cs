
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class HolidayIntegrationEventHandler :
        IIntegrationEventHandler<CreatedHolidayIntegrationEvent>,
        IIntegrationEventHandler<UpdatedHolidayIntegrationEvent>,
        IIntegrationEventHandler<DeletedHolidayIntegrationEvent>
    {
        private readonly HolidayAuditProcess _holidayAuditProcess;
        public HolidayIntegrationEventHandler(HolidayAuditProcess holidayAuditProcess)
        {
            _holidayAuditProcess = holidayAuditProcess;
        }
        public async Task Handle(CreatedHolidayIntegrationEvent @event)
        {
            await _holidayAuditProcess.WriteCreateHolidayLog(@event);
        }

        public async Task Handle(UpdatedHolidayIntegrationEvent @event)
        {
            await _holidayAuditProcess.WriteUpdateHolidayLog(@event);
        }

        public async Task Handle(DeletedHolidayIntegrationEvent @event)
        {
            await _holidayAuditProcess.WriteDeleteHolidayLog(@event);
        }
    }
}
