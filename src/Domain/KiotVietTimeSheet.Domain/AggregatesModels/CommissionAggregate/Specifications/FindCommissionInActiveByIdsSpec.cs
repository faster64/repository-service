using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionInActiveByIdsSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionInActiveByIdsSpec(ICollection<long> ids)
            : base(e => ids.Contains(e.Id) && (!e.IsActive || e.IsDeleted))
        { }
    }
}