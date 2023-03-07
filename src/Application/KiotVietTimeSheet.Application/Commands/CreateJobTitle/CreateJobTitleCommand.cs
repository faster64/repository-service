using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateJobTitle
{
    [RequiredPermission(TimeSheetPermission.JobTitle_Create)]
    public class CreateJobTitleCommand : BaseCommand<JobTitleDto>
    {
        public JobTitleDto JobTitle { get; set; }
        public CreateJobTitleCommand(JobTitleDto jobTitle)
        {
            JobTitle = jobTitle;
        }
    }
}
