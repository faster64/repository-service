using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByNotInStatusSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByNotInStatusSpec(byte status)

            : base(c => c.ClockingStatus != status)
        {
        }
    }
}
