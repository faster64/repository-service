using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Dto
{
    public class DaysOffDto
    {
        public List<Holiday> Holidays { get; set; }
        public List<BranchWorkingDayDto> BranchWorkingDays { get; set; }
    }

    public class BranchWorkingDayDto
    {
        public long BranchId { get; set; }
        public List<byte> WorkingDays { get; set; }
    }
}
