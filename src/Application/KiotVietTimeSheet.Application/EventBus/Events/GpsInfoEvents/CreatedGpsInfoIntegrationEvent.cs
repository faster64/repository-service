using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.GpsInfoEvents
{
    public class CreatedGpsInfoIntegrationEvent : IntegrationEvent
    {
        public GpsInfo GpsInfo { get; set; }
        public CreatedGpsInfoIntegrationEvent(GpsInfo gpsInfo)
        {
            GpsInfo = gpsInfo;        
        }
        [JsonConstructor]
        public CreatedGpsInfoIntegrationEvent()
        {
        }
    }
}
