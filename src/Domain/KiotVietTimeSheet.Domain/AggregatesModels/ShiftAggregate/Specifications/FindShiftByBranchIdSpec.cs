using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftByBranchIdSpec : ExpressionSpecification<Shift>
    {
        public FindShiftByBranchIdSpec(long branchId)
            : base(s => s.BranchId == branchId) { }
    }
}
