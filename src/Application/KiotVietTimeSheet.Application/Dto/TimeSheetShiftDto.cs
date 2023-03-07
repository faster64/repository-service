using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class TimeSheetShiftDto
    {
        public long Id { get; set; }
        public string ShiftIds { get; set; }
        public string RepeatDaysOfWeek { get; set; }
        public byte TimeSheetShiftStatus { get; set; }
        public TimeSheet TimeSheet { get; set; }
        public long TimeSheetId { get; set; }

    }
}
