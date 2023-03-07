using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByCategoryIdsSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByCategoryIdsSpec(List<long> categoryIds)
            : base(c => categoryIds.Contains(c.ObjectId))
        {
        }
    }
}
