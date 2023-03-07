using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Auth;

namespace KiotVietTimeSheet.Application.Commands.UpdateAllowance
{
    [RequiredPermission(TimeSheetPermission.Allowance_Update)]
    public class UpdateAllowanceCommand : BaseCommand<AllowanceDto>
    {
        public AllowanceDto Allowance { get; }

        public UpdateAllowanceCommand(AllowanceDto allowance)
        {
            Allowance = allowance;
        }
    }
}
