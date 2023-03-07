using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByClockingIdSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByClockingIdSpec(long id)
            : base(c => c.Id == id)
        {
        }
    }
}
