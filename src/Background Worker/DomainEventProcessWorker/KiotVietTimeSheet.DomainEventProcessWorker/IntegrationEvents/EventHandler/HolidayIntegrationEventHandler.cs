using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Caching;
using KiotVietTimeSheet.Application.EventBus.Events.HolidayEvents;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Common;
using KiotVietTimeSheet.DomainEventProcessWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.DomainEventProcessWorker.IntegrationEvents.EventHandler
{
    public class HolidayIntegrationEventHandler :
        IIntegrationEventHandler<CreatedHolidayIntegrationEvent>,
        IIntegrationEventHandler<UpdatedHolidayIntegrationEvent>,
        IIntegrationEventHandler<DeletedHolidayIntegrationEvent>,
        IIntegrationEventHandler<CheckUpdateTenantNationalHolidayIntegrationEvent>
    {
        private readonly HolidayAuditProcess _holidayAuditProcess;
        private readonly IKiotVietApiClient _kiotVietApiClient;
        private readonly IAppSettings _appSettings;
        private readonly ICacheClient _cacheClient;
        private readonly Helper _helper = new Helper();

        public HolidayIntegrationEventHandler(
            HolidayAuditProcess holidayAuditProcess,
            IKiotVietApiClient kiotVietApiClient, IAppSettings appSettings, ICacheClient cacheClient
        )
        {
            _holidayAuditProcess = holidayAuditProcess;
            _kiotVietApiClient = kiotVietApiClient;
            _appSettings = appSettings;
            _cacheClient = cacheClient;
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

        public async Task Handle(CheckUpdateTenantNationalHolidayIntegrationEvent @event)
        {
            using (var db = await _helper.GetDbContextByGroupId(@event.Context.GroupId,
                @event.Context.RetailerCode, _cacheClient, _kiotVietApiClient, _appSettings))
            {
                var holidays = await _helper.CreateNationalHolidayAsync(@event.Context, db, _appSettings);
                if (holidays != null && holidays.Any())
                {
                    await db.SaveChangesAsync();
                    _helper.FlushCacheStore(_cacheClient, @event.Context.RetailerCode, nameof(Holiday));
                }

            }
        }
    }
}
