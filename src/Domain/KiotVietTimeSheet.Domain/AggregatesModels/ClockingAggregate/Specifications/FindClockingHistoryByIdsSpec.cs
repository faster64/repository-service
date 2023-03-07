using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingHistoryByIdsSpec : ExpressionSpecification<ClockingHistory>
    {
        public FindClockingHistoryByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id)) { }
    }
}
