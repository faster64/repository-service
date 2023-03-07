using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByCommissionIdAndProductIdsSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByCommissionIdAndProductIdsSpec(long commissionId, List<long> productIds)
            : base(e => e.CommissionId == commissionId && productIds.Contains(e.ObjectId))
        {
        }
    }
}
