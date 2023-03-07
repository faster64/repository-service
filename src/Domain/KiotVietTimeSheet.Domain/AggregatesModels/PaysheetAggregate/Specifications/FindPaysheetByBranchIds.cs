using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByBranchIds : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByBranchIds(List<int?> branchIds)
            : base(p => branchIds.Contains(p.BranchId))
        { }
    }
}
