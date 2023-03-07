using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Events
{
    public class DeletedGpsInfoEvent : DomainEvent
    {
        public GpsInfo GpsInfo { get; set; }

        public DeletedGpsInfoEvent(GpsInfo gpsInfo)
        {
            GpsInfo = gpsInfo;
        }
    }
}
