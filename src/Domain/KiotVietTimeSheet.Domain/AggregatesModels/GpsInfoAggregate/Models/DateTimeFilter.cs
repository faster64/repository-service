using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models
{
    public class DateTimeFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TimeRange { get; set; }
    }
}
