using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class DefaultTrueSpec<TEntity> : ExpressionSpecification<TEntity>
    {
        public DefaultTrueSpec()
           : base(entity => true)
        { }
    }
}
