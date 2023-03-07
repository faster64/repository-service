using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.Common
{
    public class CheckExistsCodeSpec<TCodedEntity> : ExpressionSpecification<TCodedEntity> where TCodedEntity : BaseEntity
    {
        public CheckExistsCodeSpec(string code)
           : base(e => (e as ICode).Code.Equals(code, StringComparison.OrdinalIgnoreCase))
        { }
    }
}
