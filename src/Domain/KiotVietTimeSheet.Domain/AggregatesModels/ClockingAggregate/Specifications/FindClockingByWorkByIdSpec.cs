using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByWorkByIdSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByWorkByIdSpec(long workById)
            : base(c => c.WorkById == workById)
        {
        }
    }
}
