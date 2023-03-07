using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models
{
    public class CommissionBranch : BaseEntity, IEntityIdlong, ITenantId
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public long CommissionId { get; set; }
        public Commission Commission { get; set; }

        public CommissionBranch(int branchId, long commissionId)
        {
            BranchId = branchId;
            CommissionId = commissionId;
        }

        public CommissionBranch()
        {

        }
    }
}
