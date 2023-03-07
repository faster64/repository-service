using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications
{
    public class FindDeductionByIdsSpec : ExpressionSpecification<Deduction>
    {
        public FindDeductionByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
