using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdateClockingShiftAndDateTime
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class UpdateClockingShiftAndDateTimeCommand : BaseCommand<ClockingDto>
    {
        public long ClockingId { get; set; }
        public long ShiftTargetId { get; set; }
        public DateTime WorkingDay { get; set; }

        public UpdateClockingShiftAndDateTimeCommand(long clockingId, long shiftTargetId, DateTime workingDay)
        {
            ClockingId = clockingId;
            ShiftTargetId = shiftTargetId;
            WorkingDay = workingDay;
        }
    }
}
