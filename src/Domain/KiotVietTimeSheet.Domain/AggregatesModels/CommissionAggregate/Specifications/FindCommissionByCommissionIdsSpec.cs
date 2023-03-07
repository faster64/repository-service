using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionByCommissionIdsSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionByCommissionIdsSpec(List<long> commissionIds)
            : base(s => commissionIds.Contains(s.Id)) { }
    }
}
