using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByTenantIdSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByTenantIdSpec(int tenantId)
            : base(p => p.TenantId == tenantId)
        {
        }
    }
}
