using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Events
{
    public class UpdatedGpsInfoEvent : DomainEvent
    {
        public GpsInfo GpsInfo { get; set; }

        public UpdatedGpsInfoEvent(GpsInfo gpsInfo)
        {
            GpsInfo = gpsInfo;
        }
    }
}
