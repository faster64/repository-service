using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerPrintByBranchIdSpec : ExpressionSpecification<FingerPrint>
    {
        public FindFingerPrintByBranchIdSpec(long branchId)
            : base(x => x.BranchId == branchId)
        {
        }
    }
}
