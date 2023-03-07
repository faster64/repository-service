using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents
{
    public class DeletedGpsInfoIntegrationEvent : IntegrationEvent
    {
        public GpsInfo GpsInfo { get; set; }
        public DeletedGpsInfoIntegrationEvent(GpsInfo gpsInfo)
        {
            GpsInfo = gpsInfo;
        }
        [JsonConstructor]
        public DeletedGpsInfoIntegrationEvent()
        {
        }
    }
}
