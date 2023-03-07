using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Events
{
    public class CreatedGpsInfoEvent : DomainEvent
    {
        public GpsInfo GpsInfo { get; set; }

        public CreatedGpsInfoEvent(GpsInfo gpsInfo)
        {
            GpsInfo = gpsInfo;
        }
    }
}
