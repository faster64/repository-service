using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingPenalizeByClockingIdSpec : ExpressionSpecification<ClockingPenalize>
    {
        public FindClockingPenalizeByClockingIdSpec(long clockingId)
            : base(ch => ch.ClockingId == clockingId) { }
    }
}
