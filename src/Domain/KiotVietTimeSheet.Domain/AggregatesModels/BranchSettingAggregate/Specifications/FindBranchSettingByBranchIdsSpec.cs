using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications
{
    public class FindBranchSettingByBranchIdsSpec : ExpressionSpecification<BranchSetting>
    {
        public FindBranchSettingByBranchIdsSpec(List<int> branchIds)
            : base(s => branchIds.Contains(s.BranchId))
        { }
    }
}
