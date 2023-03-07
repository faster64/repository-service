using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications
{
    public class FindAllowanceByTenantIdSpec : ExpressionSpecification<Allowance>
    {
        public FindAllowanceByTenantIdSpec(int tenantId)
            : base(h => h.TenantId == tenantId)
        {
        }
    }
}
