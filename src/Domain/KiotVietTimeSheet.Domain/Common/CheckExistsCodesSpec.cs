using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class CheckExistsCodesSpec<TCodedEntity> : ExpressionSpecification<TCodedEntity> where TCodedEntity : BaseEntity
    {
        public CheckExistsCodesSpec(List<string> codes)
            : base(e => codes.Contains((e as ICode).Code.ToLower()))
        { }
    }
}
