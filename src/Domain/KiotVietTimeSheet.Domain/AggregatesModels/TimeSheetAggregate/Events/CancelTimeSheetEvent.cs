using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events
{
    public class CancelTimeSheetEvent : DomainEvent
    {
        public TimeSheet TimeSheet { get; set; }

        public CancelTimeSheetEvent(TimeSheet timeSheet)
        {
            TimeSheet = timeSheet;
        }
    }
}
