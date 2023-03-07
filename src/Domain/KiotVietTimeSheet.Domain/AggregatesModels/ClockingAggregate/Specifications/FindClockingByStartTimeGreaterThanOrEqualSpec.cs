using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByStartTimeGreaterThanOrEqualSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByStartTimeGreaterThanOrEqualSpec(DateTime dateTime)
            : base(c => c.StartTime >= dateTime)
        {
        }
    }
}
