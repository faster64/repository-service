using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftByBranchIdsSpec : ExpressionSpecification<Shift>
    {
        public FindShiftByBranchIdsSpec(List<int> branchIds)
            : base(s => branchIds.Contains(s.BranchId)) { }
    }
}
