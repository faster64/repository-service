using System;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.Common
{
    public class CheckExistsCodeForOrmLiteSpec<TCodedEntity> : ExpressionSpecification<TCodedEntity> where TCodedEntity : BaseEntity, ICode
    {
        public CheckExistsCodeForOrmLiteSpec(string code)
            : base(e => e.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
        { }
    }
}
