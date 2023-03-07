using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByStatusSpec : ExpressionSpecification<Clocking>
    {
        public  FindClockingByStatusSpec(byte status)

            : base(c => c.ClockingStatus == status)
        {
        }
    }
}
