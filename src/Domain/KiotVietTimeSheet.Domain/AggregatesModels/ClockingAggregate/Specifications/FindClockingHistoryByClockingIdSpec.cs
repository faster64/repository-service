using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingHistoryByClockingIdSpec : ExpressionSpecification<ClockingHistory>
    {
        public FindClockingHistoryByClockingIdSpec(long clockingId)
            : base(ch => ch.ClockingId == clockingId) { }
    }
}
