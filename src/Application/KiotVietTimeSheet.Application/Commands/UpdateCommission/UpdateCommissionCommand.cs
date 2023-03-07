using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateCommission
{
    [RequiredPermission(TimeSheetPermission.Commission_Update)]
    public class UpdateCommissionCommand : BaseCommand<CommissionDto>
    {
        public CommissionDto Commission { get; }
        public UpdateCommissionCommand(CommissionDto commission)
        {
            Commission = commission;
        }
    }
}
