using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Linq;

namespace KiotVietTimeSheet.Domain.Common
{
    public class FindByMultipleEntityIdLongSpec<TEntity> : ExpressionSpecification<TEntity>
    {
        public FindByMultipleEntityIdLongSpec(object[] keys)
          : base(entity => keys.Contains((entity as IEntityIdlong).Id))
        { }
    }
}
