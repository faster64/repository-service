using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class FilterSoftDeleteSpec<TEntity> : ExpressionSpecification<TEntity> where TEntity : class
    {
        public FilterSoftDeleteSpec()
           : base(entity => !((ISoftDelete)entity).IsDeleted)
        { }
    }
}
