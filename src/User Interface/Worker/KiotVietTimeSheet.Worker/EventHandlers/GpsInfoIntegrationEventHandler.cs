using KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents;
using KiotVietTimeSheet.AuditTrailWorker.AuditTrailProcess.Types;
using KiotVietTimeSheet.Infrastructure.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.AuditTrailWorker.EventHandlers
{
    public class GpsInfoIntegrationEventHandler :
        IIntegrationEventHandler<CreatedGpsInfoIntegrationEvent>,
        IIntegrationEventHandler<UpdatedGpsInfoIntegrationEvent>,
        IIntegrationEventHandler<DeletedGpsInfoIntegrationEvent>,
        IIntegrationEventHandler<UpdatedQrKeyIntegrationEvent>
    {
        private readonly GpsInfoAuditProcess _gpsInfoAuditProcess;
        private readonly ILogger<GpsInfoIntegrationEventHandler> _logger;
        public GpsInfoIntegrationEventHandler(GpsInfoAuditProcess gpsInfoAuditProcess, ILogger<GpsInfoIntegrationEventHandler> logger)
        {
            _gpsInfoAuditProcess = gpsInfoAuditProcess;
            _logger = logger;
        }
        public async Task Handle(CreatedGpsInfoIntegrationEvent @event)
        {
            _logger.LogDebug($"Handle event {nameof(GpsInfoIntegrationEventHandler)}");
            await _gpsInfoAuditProcess.WriteCreateGpsInfoAsync(@event);
        }

        public async Task Handle(UpdatedGpsInfoIntegrationEvent @event)
        {
            _logger.LogDebug($"Handle event {nameof(GpsInfoIntegrationEventHandler)}");
            await _gpsInfoAuditProcess.WriteUpdateGpsInfoAsync(@event);
        }

        public async Task Handle(DeletedGpsInfoIntegrationEvent @event)
        {
            _logger.LogDebug($"Handle event {nameof(GpsInfoIntegrationEventHandler)}");
            await _gpsInfoAuditProcess.WriteDeleteGpsInfoAsync(@event);
        }

        public async Task Handle(UpdatedQrKeyIntegrationEvent @event)
        {
            await _gpsInfoAuditProcess.WriteUpdateQrKeyAsync(@event);
        }
    }
}
