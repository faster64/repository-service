using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class StartWithCodePrefixSpec<TCodedEntity> : ExpressionSpecification<TCodedEntity> where TCodedEntity : BaseEntity
    {
        public StartWithCodePrefixSpec(string codePrefix)
           : base(entity => (entity as ICode).Code.StartsWith(codePrefix))
        {

        }
    }
}
