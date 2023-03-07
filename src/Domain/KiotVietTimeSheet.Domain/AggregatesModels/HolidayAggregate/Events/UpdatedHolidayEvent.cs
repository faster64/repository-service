using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events
{
    public class UpdatedHolidayEvent : DomainEvent
    {
        public Holiday OldHoliday { get; set; }
        public Holiday NewHoliday { get; set; }

        public UpdatedHolidayEvent(Holiday oldHoliday, Holiday newHoliday)
        {
            OldHoliday = oldHoliday;
            NewHoliday = newHoliday;
        }
    }
}
