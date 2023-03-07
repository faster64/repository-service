using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.RejectClockingMultiple
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class RejectClockingMultipleCommand : BaseCommand<List<ClockingDto>>
    {
        public List<long> ListClockingId { get; set; }

        public RejectClockingMultipleCommand(List<long> listClockingId)
        {
            ListClockingId = listClockingId;
        }
    }
}