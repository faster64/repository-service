using System;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Utilities;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsRuleValue(this Type type)
        {
            return type.HasInterface(typeof(IRuleValue));
        }

        public static bool IsRuleParam(this Type type)
        {
            return type.HasInterface(typeof(IRuleParam));
        }
    }
}
