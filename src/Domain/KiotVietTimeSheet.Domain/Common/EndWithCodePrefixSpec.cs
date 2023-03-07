using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class EndWithCodePrefixSpec<TCodedEntity> : ExpressionSpecification<TCodedEntity> where TCodedEntity : BaseEntity
    {
        public EndWithCodePrefixSpec(string codePrefix)
            : base(entity => (entity as ICode).Code.EndsWith(codePrefix))
        {

        }
    }
}
