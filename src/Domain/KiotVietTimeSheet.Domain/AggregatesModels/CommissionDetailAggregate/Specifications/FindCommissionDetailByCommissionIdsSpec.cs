using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByCommissionIdsSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByCommissionIdsSpec(List<long> commissionIds)
            : base(e => commissionIds.Contains(e.CommissionId))
        {
        }
    }
}
