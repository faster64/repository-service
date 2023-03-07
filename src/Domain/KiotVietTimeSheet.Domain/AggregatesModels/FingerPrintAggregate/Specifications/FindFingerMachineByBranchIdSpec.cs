
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerMachineByBranchIdSpec : ExpressionSpecification<FingerMachine>
    {
        public FindFingerMachineByBranchIdSpec(long branchId)
            : base(x => x.BranchId == branchId)
        {
        }
    }
}
