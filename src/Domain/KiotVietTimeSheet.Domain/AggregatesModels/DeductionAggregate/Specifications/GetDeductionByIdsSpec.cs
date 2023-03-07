using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications
{
    public class GetDeductionByIdsSpec : ExpressionSpecification<Deduction>
    {
        public GetDeductionByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        {

        }
    }
}
