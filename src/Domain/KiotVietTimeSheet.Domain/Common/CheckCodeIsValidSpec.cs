using System;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;

namespace KiotVietTimeSheet.Domain.Common
{
    public class CheckCodeIsValidSpec<ICodeEntity> : ExpressionSpecification<ICodeEntity> where ICodeEntity : BaseEntity
    {
        private static bool IsNumeric(string str)
        {
            int num;
            return Int32.TryParse(str, out num);
        }

        public CheckCodeIsValidSpec(int length, string prefix, string suffix)
           : base(e => IsNumeric((e as ICode).Code.Substring(prefix.Length, length - prefix.Length - suffix.Length)))
        {

        }
    }
}
