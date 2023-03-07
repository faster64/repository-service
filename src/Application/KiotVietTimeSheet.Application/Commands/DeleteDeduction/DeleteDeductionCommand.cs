using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Commands.DeleteDeduction
{
    [RequiredPermission(TimeSheetPermission.Deduction_Update)]
    public class DeleteDeductionCommand : BaseCommand
    {
        public long Id { get; }
        public DeleteDeductionCommand(long id)
        {
            Id = id;
        }
    }
}
