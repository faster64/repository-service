using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class FindByEntityIdLongSpec<TEntity> : ExpressionSpecification<TEntity>
    {
        public FindByEntityIdLongSpec(long id)
          : base(entity => ((IEntityIdlong)entity).Id == id)
        { }
    }
}
