using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionByIdsSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionByIdsSpec(ICollection<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
