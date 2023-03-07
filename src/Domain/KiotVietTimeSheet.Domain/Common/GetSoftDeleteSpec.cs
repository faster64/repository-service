using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class GetSoftDeleteSpec<TEntity> : ExpressionSpecification<TEntity>
    {
        public GetSoftDeleteSpec()
           : base(entity => (entity as ISoftDelete).IsDeleted == true)
        { }
    }
}
