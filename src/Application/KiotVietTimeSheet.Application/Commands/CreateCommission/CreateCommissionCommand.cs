using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateCommission
{
    [RequiredPermission(TimeSheetPermission.Commission_Create)]
    public class CreateCommissionCommand : BaseCommand<CommissionDto>
    {
        public CommissionDto Commission { get; }
        public CreateCommissionCommand(CommissionDto commission)
        {
            Commission = commission;
        }
    }
}
