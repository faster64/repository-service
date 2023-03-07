using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class LimitByCodeLengthSpec<TEntity> : ExpressionSpecification<TEntity>
    {
        public LimitByCodeLengthSpec(int len)
           : base(e => (e as ICode).Code.Length == len)
        {

        }
    }
}
