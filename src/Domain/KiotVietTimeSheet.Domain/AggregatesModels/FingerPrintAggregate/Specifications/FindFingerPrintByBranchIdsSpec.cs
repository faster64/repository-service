using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerPrintByBranchIdsSpec : ExpressionSpecification<FingerPrint>
    {
        public FindFingerPrintByBranchIdsSpec(List<int> branchIds)
            : base(x => branchIds.Contains(x.BranchId))
        {
        }
    }
}