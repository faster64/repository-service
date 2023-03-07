using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindActiveCommissionByIdsSpec : ExpressionSpecification<Commission>
    {
        public FindActiveCommissionByIdsSpec(ICollection<long> ids)
            : base(e => ids.Contains(e.Id) && e.IsActive)
        { }
    }
}
