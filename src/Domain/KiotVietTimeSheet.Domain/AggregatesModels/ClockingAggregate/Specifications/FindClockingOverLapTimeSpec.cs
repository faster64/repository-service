using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;


namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingOverLapTimeSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingOverLapTimeSpec(DateTime startTime, DateTime endTime)
            : base(x => x.StartTime < endTime && x.EndTime > startTime)
        {
        }
    }
}
