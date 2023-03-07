using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByStartTimeLessThanOrEqualSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByStartTimeLessThanOrEqualSpec(DateTime dateTime)
            : base(c => c.StartTime <= dateTime)
        {
        }
    }
}
