using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByStartTimeLessThanSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByStartTimeLessThanSpec(DateTime dateTime)
            : base(c => c.StartTime < dateTime)
        {
        }
    }
}
