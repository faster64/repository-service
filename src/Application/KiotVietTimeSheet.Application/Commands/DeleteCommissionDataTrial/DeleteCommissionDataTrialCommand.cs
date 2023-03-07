using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.DeleteCommissionDataTrial
{
    public class DeleteCommissionDataTrialCommand : BaseCommand<CommissionDto>, IInternalRequest
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public long UserIdAdmin { get; set; }
        public string TenantCode { get; set; }
        public int TenantId { get; set; }
        public int GroupId { get; set; }

        public DeleteCommissionDataTrialCommand(long id, int branchId, long userIdAdmin, string tenantCode, int tenantId, int groupId)
        {
            Id = id;
            UserIdAdmin = userIdAdmin;
            TenantCode = tenantCode;
            TenantId = tenantId;
            GroupId = groupId;
            BranchId = branchId;
        }
    }
}
