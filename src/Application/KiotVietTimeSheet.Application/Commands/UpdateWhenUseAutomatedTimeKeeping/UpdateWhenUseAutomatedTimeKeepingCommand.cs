using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using System;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdateWhenUseAutomatedTimeKeeping
{
    [RequiredPermission(TimeSheetPermission.Clocking_Update)]
    public class UpdateWhenUseAutomatedTimeKeepingCommand : BaseCommand<ClockingDto>
    {
        public long EmployeeId { get; set; }
        public DateTime TimeKeepingDate { get; set; }

        public UpdateWhenUseAutomatedTimeKeepingCommand(long employeeId, DateTime timeKeepingDate)
        {
            EmployeeId = employeeId;
            TimeKeepingDate = timeKeepingDate;
        }
    }
}