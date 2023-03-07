using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByProductIdsSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByProductIdsSpec(List<long> productIds)
            : base(c => productIds.Contains(c.ObjectId) && c.Type == (byte)CommissionDetailType.Product)
        {
        }
    }
}
