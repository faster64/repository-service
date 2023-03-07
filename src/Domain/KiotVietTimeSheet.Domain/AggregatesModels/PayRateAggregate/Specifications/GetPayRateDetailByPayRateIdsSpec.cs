using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications
{
    public class GetPayRateDetailByPayRateIdsSpec : ExpressionSpecification<PayRateDetail>
    {
        public GetPayRateDetailByPayRateIdsSpec(List<long> payRateIds)
            : base(p => payRateIds.Contains(p.PayRateId))
        {
        }
    }
}
