using KiotVietTimeSheet.Application.Abstractions;

namespace KiotVietTimeSheet.Application.Commands.DeleteTrialData
{
    public class DeleteTrialDataCommand : BaseCommand<object>
    {
        public int BranchId { get; set; }
        public int TrialType { get; set; }
        public long UserIdAdmin { get; set; }
        public string TenantCode { get; set; }
        public int TenantId { get; set; }
        public int GroupId { get; set; }

        public DeleteTrialDataCommand(int trialType, int branchId, long userIdAdmin, string tenantCode, int tenantId, int groupId)
        {
            BranchId = branchId;
            TrialType = trialType;
            UserIdAdmin = userIdAdmin;
            TenantCode = tenantCode;
            TenantId = tenantId;
            GroupId = groupId;
        }
    }
}