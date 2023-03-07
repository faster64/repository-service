using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateTimesheet
{
    [RequiredPermission(TimeSheetPermission.Clocking_Create)]
    public class CreateTimesheetCommand : BaseCommand<TimeSheetDto>
    {
        public TimeSheetDto TimeSheet { get; set; }

        public CreateTimesheetCommand(TimeSheetDto timeSheet)
        {
            TimeSheet = timeSheet;
        }
    }
}
