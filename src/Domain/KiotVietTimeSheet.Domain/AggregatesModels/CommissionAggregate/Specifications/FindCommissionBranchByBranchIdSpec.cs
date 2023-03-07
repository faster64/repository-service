using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionBranchByBranchIdSpec : ExpressionSpecification<CommissionBranch>
    {
        public FindCommissionBranchByBranchIdSpec(long branchId)
            : base(s => s.BranchId == branchId) { }
    }
}
