using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionByTenantIdSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionByTenantIdSpec(int tenantId)
            : base(s => s.TenantId == tenantId) { }
    }
}
