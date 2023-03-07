using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteJobTitle
{
    [RequiredPermission(TimeSheetPermission.JobTitle_Delete)]
    public class DeleteJobTitleCommand : BaseCommand
    {
        public long Id { get; set; }

        public DeleteJobTitleCommand(long id)
        {
            Id = id;
        }
    }
}
