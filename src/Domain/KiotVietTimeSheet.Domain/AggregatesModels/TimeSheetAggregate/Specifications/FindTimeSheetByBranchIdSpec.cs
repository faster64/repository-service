using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByBranchIdSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByBranchIdSpec(int branchId)
            : base(c => c.BranchId == branchId)
        {
        }
    }
}
