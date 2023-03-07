using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events
{
    public class CreatedTimeSheetEvent : DomainEvent
    {
        public TimeSheet TimeSheet { get; set; }
        public CreatedTimeSheetEvent(TimeSheet timeSheet)
        {
            TimeSheet = timeSheet;
        }
    }
}
