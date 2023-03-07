using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.DeleteCommission
{
    [RequiredPermission(TimeSheetPermission.Commission_Delete)]
    public class DeleteCommissionCommand : BaseCommand<CommissionDto>
    {
        public long Id { get; set; }

        public DeleteCommissionCommand(long id)
        {
            Id = id;
        }
    }
}
