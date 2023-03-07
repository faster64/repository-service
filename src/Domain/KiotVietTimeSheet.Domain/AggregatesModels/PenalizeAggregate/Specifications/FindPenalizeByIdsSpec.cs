using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications
{
    public class FindPenalizeByIdsSpec : ExpressionSpecification<Penalize>
    {
        public FindPenalizeByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
