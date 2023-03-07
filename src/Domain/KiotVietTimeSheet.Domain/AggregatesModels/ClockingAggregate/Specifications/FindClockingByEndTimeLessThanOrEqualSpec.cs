using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByEndTimeLessThanOrEqualSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByEndTimeLessThanOrEqualSpec(DateTime dateTime)
            : base(c => c.EndTime <= dateTime)
        {
        }
    }
}
