using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications
{
    public class GetPayRateByIdsSpecification : ExpressionSpecification<PayRate>
    {
        public GetPayRateByIdsSpecification(List<long> Ids)
            : base(e => Ids.Contains(e.Id))
        { }
    }
}
