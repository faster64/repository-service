using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KiotVietTimeSheet.SharedKernel.Specifications;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Utilities
{
    public static class CacheHelpers
    {
        public static string GetObjectHashValue<T>(object obj, ISpecification<T> guardDataAccessSpecification)
        {
            return EncryptHelpers.GetHashString(GetPublicPropertyValues<T>(obj, guardDataAccessSpecification));
        }

        private static string GetPublicPropertyValues<T>(object obj, ISpecification<T> guardDataAccessSpecification)
        {
            var specificationStr = string.Empty;
            var sqlExpressionPublicProperties = string.Empty;
            var sqlExpressionParamsStr = string.Empty;

            if (obj == null)
            {
                return ConvertExpressionToString(guardDataAccessSpecification.GetExpression());
            }

            if (obj.GetType().HasInterface(typeof(ISpecification<T>)))
            {
                var specification = ((ISpecification<T>) obj).And(guardDataAccessSpecification);
                specificationStr = ConvertExpressionToString(specification.GetExpression());
            }
            // Only for IQueryExpression because we use AutoQueryFeature
            else if (obj.GetType().HasInterface(typeof(ISqlExpression)) && ((ISqlExpression) obj).Params != null)
            {
                specificationStr = ConvertExpressionToString(guardDataAccessSpecification.GetExpression());
                sqlExpressionPublicProperties = string.Join("-", obj.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(v => $"{v.GetValue(obj)}")
                    .ToArray());

                sqlExpressionParamsStr = string.Join(",",
                    ((ISqlExpression) obj).Params.Select(p => p.SqlValue()).ToArray());
            }

            return $"{specificationStr}{sqlExpressionPublicProperties}{sqlExpressionParamsStr}";
        }

        private static string ConvertExpressionToString<T>(Expression<Func<T, bool>> expression)
        {
            var specificationStr = expression.Body.ToString();

            var funcDelegate = expression.Compile();
            var constants = (object[])funcDelegate.Target.GetType().GetField("Constants")
                .GetValue(funcDelegate.Target);

            foreach (var constant in constants)
            {
                foreach (var field in constant.GetType().GetFields())
                {
                    var fullName = constant.ToString();
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        specificationStr =
                            specificationStr.Replace(fullName, field.GetValue(constant).SqlValue());
                    }
                }
            }

            return specificationStr;
        }
    }
}
