using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionBranchByCommissionIdSpec : ExpressionSpecification<CommissionBranch>
    {
        public FindCommissionBranchByCommissionIdSpec(long commissionId)
            : base(s => s.CommissionId == commissionId) { }
    }
}
