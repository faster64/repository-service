using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents
{
    public class UpdatedGpsInfoIntegrationEvent : IntegrationEvent
    {
        public GpsInfo GpsInfoNew { get; set; }
        public GpsInfo GpsInfoOld { get; set; }
        public UpdatedGpsInfoIntegrationEvent(GpsInfo gpsInfoNew, GpsInfo gpsInfoOld)
        {
            GpsInfoNew = gpsInfoNew;
            GpsInfoOld = gpsInfoOld;
        }
        [JsonConstructor]
        public UpdatedGpsInfoIntegrationEvent()
        {
        }
    }
}
