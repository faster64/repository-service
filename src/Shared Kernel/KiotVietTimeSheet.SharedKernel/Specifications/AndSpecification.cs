using System;
using System.Linq.Expressions;

namespace KiotVietTimeSheet.SharedKernel.Specifications
{
    public class AndSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _leftSpec;
        private readonly ISpecification<T> _rightSpec;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _leftSpec = left;
            _rightSpec = right;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            var leftExpression = _leftSpec.GetExpression();
            var rightExpression = _rightSpec.GetExpression();
            var paramExpr = Expression.Parameter(typeof(T));
            var andExpression = Expression.AndAlso(leftExpression.Body, rightExpression.Body);

            andExpression = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(andExpression);
            var finalExpr = Expression.Lambda<Func<T, bool>>(andExpression, paramExpr);

            return finalExpr;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return _leftSpec.IsSatisfiedBy(o) && _rightSpec.IsSatisfiedBy(o);
        }
    }
}
