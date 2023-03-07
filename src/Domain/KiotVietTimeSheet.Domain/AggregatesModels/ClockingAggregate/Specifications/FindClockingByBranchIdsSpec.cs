using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByBranchIdsSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByBranchIdsSpec(ICollection<int> branchIds)
            : base(c => branchIds.Contains(c.BranchId))
        {
        }
    }
}
