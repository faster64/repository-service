using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByShiftIdSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByShiftIdSpec(long shiftId)
            : base(c => c.ShiftId == shiftId)
        {
        }
    }
}
