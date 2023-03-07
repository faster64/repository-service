using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System;

namespace KiotVietTimeSheet.Application.Commands.UpdateClockingMultipleBranchShiftAndDateTime
{
    public class UpdateClockingMultipleBranchShiftAndDateTimeCommand : BaseCommand<ClockingDto>
    {
        public long ClockingId { get; set; }
        public long ShiftTargetId { get; set; }
        public DateTime WorkingDay { get; set; }
        public int BranchId { get; set; }

        public UpdateClockingMultipleBranchShiftAndDateTimeCommand(long clockingId, long shiftTargetId, int branchId, DateTime workingDay)
        {
            ClockingId = clockingId;
            ShiftTargetId = shiftTargetId;
            WorkingDay = workingDay;
            BranchId = branchId;
        }
    }
}
