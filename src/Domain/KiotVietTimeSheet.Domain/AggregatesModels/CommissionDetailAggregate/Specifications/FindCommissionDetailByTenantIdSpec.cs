using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByTenantIdSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByTenantIdSpec(int tenantId)
            : base(c => c.TenantId == tenantId)
        {
        }
    }
}
