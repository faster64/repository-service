using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class TimeSheetDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsRepeat { get; set; }
        public byte? RepeatType { get; set; }
        public byte? RepeatEachDay { get; set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public bool SaveOnDaysOffOfBranch { get; set; }
        public bool SaveOnHoliday { get; set; }
        public byte TimeSheetStatus { get; set; }
        public List<Clocking> Clockings { get; set; }
        public List<ClockingDto> ClockingsOverlapTime { get; set; }
        public List<TimeSheetShiftDto> TimeSheetShifts { get; set; }
        public string Note { get; set; }
        public Employee Employee { get; set; }
        public bool HasEndDate { get; set; }
    }
}
