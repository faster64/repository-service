using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingPenalizeByPenalizeIdsSpec : ExpressionSpecification<ClockingPenalize>
    {
        public FindClockingPenalizeByPenalizeIdsSpec(List<long> penlaizeIds)
            : base(ch => penlaizeIds.Contains(ch.PenalizeId)) { }
    }
}
