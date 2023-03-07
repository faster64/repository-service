using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events
{
    public class CreatedHolidayEvent : DomainEvent
    {
        public Holiday Holiday { get; set; }

        public CreatedHolidayEvent(Holiday holiday)
        {
            Holiday = holiday;
        }
    }
}
