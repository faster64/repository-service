using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteAllowance
{
    [RequiredPermission(TimeSheetPermission.Allowance_Update)]
    public class DeleteAllowanceCommand : BaseCommand
    {
        public long Id { get; }

        public DeleteAllowanceCommand(long id)
        {
            Id = id;
        }
    }
}
