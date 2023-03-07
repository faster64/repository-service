using System.Collections.Generic;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Dto
{
    public class GenerateClockingForTimeSheetsDto
    {
        public List<TimeSheet> TimeSheets { get; set; }
        public bool IsAddClockings { get; set; } = true;
        public bool IsRemoveClockings { get; set; }
        /// <summary>
        /// Thời gian áp dụng thay đổi chi tiết làm việc
        /// </summary>
        public List<DateRangeDto> ApplyTimes { get; set; }
        public List<Holiday> Holidays { get; set; }
        public List<byte> WorkingDays { get; set; }
        public List<byte> WorkingDaysChanged { get; set; }
        public GenerateClockingByType? GenerateByType { get; set; }
    }
}
