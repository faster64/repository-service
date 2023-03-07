using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionBranchByCommissionIdsSpec : ExpressionSpecification<CommissionBranch>
    {
        public FindCommissionBranchByCommissionIdsSpec(List<long> commissionIds)
            : base(s => commissionIds.Contains(s.CommissionId)) { }
    }
}
