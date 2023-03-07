using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Specifications
{
    public class FindPayRateTemplateByBranchIdSpec : ExpressionSpecification<PayRateTemplate>
    {
        public FindPayRateTemplateByBranchIdSpec(int branchId)
            : base(p => p.BranchId == branchId)
        { }
    }
}
