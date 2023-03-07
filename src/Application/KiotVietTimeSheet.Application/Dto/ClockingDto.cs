using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ClockingDto
    {
        public long Id { get; set; }
        public long ShiftId { get; set; }
        public ShiftDto Shift { get; set; }
        public long TimeSheetId { get; set; }
        public long EmployeeId { get; set; }
        public long WorkById { get; set; }
        public byte ClockingStatus { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Note { get; set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public Employee Employee { get; set; }
        public Employee WorkBy { get; set; }
        public TimeSheetDto TimeSheet { get; set; }
        public List<ClockingHistory> ClockingHistories { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int TimeIsLate { get; set; }
        public int OverTimeBeforeShiftWork { get; set; }
        public int TimeIsLeaveWorkEarly { get; set; }
        public int OverTimeAfterShiftWork { get; set; }
        public byte? AbsenceType { get; set; }
        public int? OverlapType { get; set; }
        public string OverlapHolidayName { get; set; }
        public byte ClockingPaymentStatus { get; set; }
        public int? ClockingDayType { get; set; }
        public List<ClockingPenalizeDto> ClockingPenalizesDto { get; set; }
    }
}
