using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateAllowance
{
    [RequiredPermission(TimeSheetPermission.Allowance_Update)]
    public class CreateAllowanceCommand : BaseCommand<AllowanceDto>
    {
        public AllowanceDto Allowance { get; set; }

        public CreateAllowanceCommand(AllowanceDto allowance)
        {
            Allowance = allowance;
        }
    }
}
