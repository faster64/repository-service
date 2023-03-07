using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class LimitByTenantIdSpec<TEntity> : ExpressionSpecification<TEntity> where TEntity : class
    {
        public LimitByTenantIdSpec(int tenantId)
           : base(e => ((ITenantId)e).TenantId == tenantId)
        {

        }
    }
}
