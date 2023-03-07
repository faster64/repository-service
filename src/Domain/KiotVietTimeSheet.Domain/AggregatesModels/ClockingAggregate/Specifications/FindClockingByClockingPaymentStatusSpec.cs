using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByClockingPaymentStatusSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByClockingPaymentStatusSpec(byte clockingPaymentStatus)
            : base(c => c.ClockingPaymentStatus == clockingPaymentStatus) { }
    }
}
