using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByBranchIdSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByBranchIdSpec(long branchId)
            : base(c => c.BranchId == branchId)
        { }
    }
}
