using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications
{
    public class FindDeductionByTenantIdSpec : ExpressionSpecification<Deduction>
    {
        public FindDeductionByTenantIdSpec(int tenantId)
            : base(h => h.TenantId == tenantId)
        {
        }
    }
}
