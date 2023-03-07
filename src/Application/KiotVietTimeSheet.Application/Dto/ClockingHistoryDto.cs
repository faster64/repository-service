using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;

namespace KiotVietTimeSheet.Application.Dto
{
    public class ClockingHistoryDto
    {
        public long Id { get; set; }
        public long ClockingId { get; set; }
        public DateTime? CheckedInDate { get; set; }
        public DateTime? CheckedOutDate { get; set; }
        public int TimeIsLate { get; set; }
        public int OverTimeAfterShiftWork { get; set; }
        public int OverTimeBeforeShiftWork { get; set; }
        public int TimeIsLeaveWorkEarly { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public byte TimeKeepingType { get; set; }
        public byte ClockingStatus { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte? AbsenceType { get; set; }
        public DateTime? CheckTime { get; set; }
        public ClockingHistoryType CheckInDateType { get; set; }
        public ClockingHistoryType? CheckOutDateType { get; set; }
    }
}
