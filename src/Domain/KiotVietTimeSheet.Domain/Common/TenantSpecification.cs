using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public sealed class TenantSpecification : ExpressionSpecification<ITenantId>
    {
        public TenantSpecification(int tenantId, bool defaultIfEmpty = true)
           : base(entity => entity is ITenantId ? entity.TenantId == tenantId : defaultIfEmpty)
        {

        }
    }
}
