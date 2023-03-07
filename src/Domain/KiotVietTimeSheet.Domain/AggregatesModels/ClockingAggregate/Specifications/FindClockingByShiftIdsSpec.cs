using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByShiftIdsSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByShiftIdsSpec(List<long> shiftIds)
            : base(c => shiftIds.Contains(c.ShiftId))
        { }
    }
}
