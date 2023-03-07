using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByStatusesSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByStatusesSpec(List<byte> statuses)
            : base(c => statuses.Contains(c.ClockingStatus))
        {
        }
    }
}
