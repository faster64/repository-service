using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using System.Threading.Tasks;
using KiotVietTimeSheet.BackgroundTasks.IntegrationEvents.Events;
using KiotVietTimeSheet.BackgroundTasks.BackgroundProcess.Types;

namespace KiotVietTimeSheet.BackgroundTasks.EventHandlers
{
    public class HolidayIntegrationEventHandler : IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>
    {
        private readonly HolidayProcess _holidayProcess;

        public HolidayIntegrationEventHandler(HolidayProcess holidayProcess)
        {
            _holidayProcess = holidayProcess;
        }

        public async Task Handle(CheckUpdateTenantNationalHolidayIntegrationEvent @event)
        {
            await _holidayProcess.CheckUpdateTenantNationHoliday(@event);
        }
    }
}
