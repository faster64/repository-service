using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Specifications
{
    public class FindBranchSettingByBranchIdSpec : ExpressionSpecification<BranchSetting>
    {
        public FindBranchSettingByBranchIdSpec(int branchId)
            : base((s) => s.BranchId == branchId)
        { }
    }
}
