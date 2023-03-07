using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events
{
    public class DeletedHolidayEvent : DomainEvent
    {
        public Holiday Holiday { get; set; }

        public DeletedHolidayEvent(Holiday holiday)
        {
            Holiday = holiday;
        }
    }
}
