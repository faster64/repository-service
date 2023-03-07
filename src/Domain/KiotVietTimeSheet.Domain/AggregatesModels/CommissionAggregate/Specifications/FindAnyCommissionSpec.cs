using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindHasAnyCommissionByTenantIdSpec : ExpressionSpecification<Commission>
    {
        public FindHasAnyCommissionByTenantIdSpec(int tenantId, bool isDeleted)
            : base(s => s.TenantId == tenantId && !s.IsDeleted) { }
    }
}
