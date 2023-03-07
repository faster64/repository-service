using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateConfirmClocking
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Update)]
    public class UpdateConfirmClockingCommand : BaseCommand<List<ConfirmClockingDto>>
    {
        public List<ConfirmClockingDto> LsConfirmClockingDto { get; set; }
        public UpdateConfirmClockingCommand(List<ConfirmClockingDto> lsConfirmClockingDto)
        {
            this.LsConfirmClockingDto = lsConfirmClockingDto;
        }

    }
}
