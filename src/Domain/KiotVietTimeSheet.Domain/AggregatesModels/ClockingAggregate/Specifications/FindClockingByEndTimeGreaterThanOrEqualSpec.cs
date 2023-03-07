using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByEndTimeGreaterThanOrEqualSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByEndTimeGreaterThanOrEqualSpec(DateTime dateTime)
            : base(c => c.EndTime >= dateTime)
        {
        }
    }
}
