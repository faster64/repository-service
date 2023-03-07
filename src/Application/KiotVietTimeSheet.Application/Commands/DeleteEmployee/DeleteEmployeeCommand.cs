using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteEmployee
{
    [RequiredPermission(TimeSheetPermission.Employee_Delete)]
    public class DeleteEmployeeCommand : BaseCommand
    {
        public long Id { get; set; }
        public DeleteEmployeeCommand(long id)
        {
            Id = id;
        }
    }

    public class DeleteSyncEmployeeCommand : BaseCommand, IInternalRequest
    {
        public long Id { get; set; }

        public DeleteSyncEmployeeCommand(long id)
        {
            Id = id;
        }
    }
}
