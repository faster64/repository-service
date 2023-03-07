using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByIdsSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
