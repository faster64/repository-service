using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events
{
    public class UpdatedTimeSheetEvent : DomainEvent
    {
        public TimeSheet TimeSheet { get; set; }
        public TimeSheet OldTimeSheet { get; set; }

        public UpdatedTimeSheetEvent(TimeSheet oldTimeSheet, TimeSheet timeSheet)
        {
            TimeSheet = timeSheet;
            OldTimeSheet = oldTimeSheet;
        }
    }
}
