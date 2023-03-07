using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByIdsSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByIdsSpec(List<long> ids)
            : base(c => ids.Contains(c.Id))
        {
        }
    }
}
