using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateTimesheet
{
    [RequiresAnyPermission(TimeSheetPermission.Clocking_Create, TimeSheetPermission.Clocking_Update)]
    public class UpdateTimesheetCommand : BaseCommand<TimeSheetDto>
    {
        public TimeSheetDto TimeSheet { get; set; }
        public bool ForAllClockings { get; set; }

        public UpdateTimesheetCommand(TimeSheetDto timeSheet, bool forAllClockings)
        {
            TimeSheet = timeSheet;
            ForAllClockings = forAllClockings;
        }
    }
}
