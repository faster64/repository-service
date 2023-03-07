using KiotVietTimeSheet.Application.Abstractions;

namespace KiotVietTimeSheet.Application.Commands.CreateTrialData
{
    public class CreateTrialDataCommand : BaseCommand<object>, IInternalRequest
    {
        public int TrialType { get; set; }
        public int BranchId { get; set; }
        public long UserId1 { get; set; }
        public long UserId2 { get; set; }
        public long UserIdAdmin { get; set; }
        public string TenantCode { get; set; }
        public int TenantId { get; set; }
        public int GroupId { get; set; }

        public CreateTrialDataCommand(int trialType, int branchId, long userId1, long userId2, long userIdAdmin, string tenantCode, int tenantId, int groupId)
        {
            TrialType = trialType;
            BranchId = branchId;
            UserId1 = userId1;
            UserId2 = userId2;
            UserIdAdmin = userIdAdmin;
            TenantCode = tenantCode;
            TenantId = tenantId;
            GroupId = groupId;
        }
    }
}