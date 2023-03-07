using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateClocking
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class UpdateClockingCommand : BaseCommand<ClockingDto>
    {
        public ClockingDto ClockingDto { get; set; }
        public long ReplacementEmployeeId { get; set; }
        public ClockingHistoryDto ClockingHistoryDto { get; set; }
        public bool LeaveOfAbsence { get; set; }

        public UpdateClockingCommand(ClockingDto clockingDto, long replacementEmployeeId, ClockingHistoryDto clockingHistoryDto, bool leaveOfAbsence)
        {
            ClockingDto = clockingDto;
            ReplacementEmployeeId = replacementEmployeeId;
            ClockingHistoryDto = clockingHistoryDto;
            LeaveOfAbsence = leaveOfAbsence;
        }
    }
}