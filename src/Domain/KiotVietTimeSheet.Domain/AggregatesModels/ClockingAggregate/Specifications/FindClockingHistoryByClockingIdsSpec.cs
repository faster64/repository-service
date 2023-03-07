using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingHistoryByClockingIdsSpec : ExpressionSpecification<ClockingHistory>
    {
        public FindClockingHistoryByClockingIdsSpec(List<long> clockingIds)
            : base(ch => clockingIds.Contains(ch.ClockingId)) { }
    }
}
