using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.UpdateJobTitle
{
    [RequiredPermission(TimeSheetPermission.JobTitle_Update)]
    public class UpdateJobTitleCommand : BaseCommand
    {
        public JobTitleDto JobTitle { get; set; }
        public UpdateJobTitleCommand(JobTitleDto jobTitle)
        {
            JobTitle = jobTitle;
        }
    }
}
