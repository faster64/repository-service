using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.BatchUpdateWhenUseAutomatedTimeKeeping
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class BatchUpdateWhenUseAutomatedTimeKeepingCommand : BaseCommand<List<ClockingDto>>
    {
        public List<AutomatedTimekeepingDto> AutomatedTimekeepingDtos { get; set; }

        public BatchUpdateWhenUseAutomatedTimeKeepingCommand(List<AutomatedTimekeepingDto> automatedTimekeepingDtos)
        {
            AutomatedTimekeepingDtos = automatedTimekeepingDtos;
        }
    }
}