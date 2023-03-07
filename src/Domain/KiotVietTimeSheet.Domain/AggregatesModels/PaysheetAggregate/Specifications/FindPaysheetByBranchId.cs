using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByBranchId : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByBranchId(int branchId)
            : base(e => e.BranchId == branchId)
        { }
    }
}
