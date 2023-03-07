using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.BatchUpdateClocking
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class BatchUpdateClockingCommand : BaseCommand<List<ClockingDto>>
    {
        public List<ClockingDto> ClockingDtos { get; set; }
        public ClockingHistoryDto ClockingHistoryDto { get; set; }
        public bool LeaveOfAbsence { get; set; }

        public BatchUpdateClockingCommand(List<ClockingDto> clockingDtos, ClockingHistoryDto clockingHistoryDto,
            bool leaveOfAbsence)
        {
            ClockingDtos = clockingDtos;
            ClockingHistoryDto = clockingHistoryDto;
            LeaveOfAbsence = leaveOfAbsence;
        }
    }
}
