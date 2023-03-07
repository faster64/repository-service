using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications
{
    public class FindPenalizeByTenantIdSpec : ExpressionSpecification<Penalize>
    {
        public FindPenalizeByTenantIdSpec(int tenantId)
            : base(h => h.TenantId == tenantId)
        {
        }
    }
}
