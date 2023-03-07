using System;
using System.Linq.Expressions;

namespace KiotVietTimeSheet.SharedKernel.Specifications
{
    public class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _leftSpec;
        private readonly ISpecification<T> _rightSpec;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _leftSpec = left;
            _rightSpec = right;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            var leftExpression = _leftSpec.GetExpression();
            var rightExpression = _rightSpec.GetExpression();
            var paramExpr = Expression.Parameter(typeof(T));
            var orExpression = Expression.OrElse(leftExpression.Body, rightExpression.Body);

            orExpression = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(orExpression);
            var finalExpr = Expression.Lambda<Func<T, bool>>(orExpression, paramExpr);

            return finalExpr;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return _leftSpec.IsSatisfiedBy(o) || _rightSpec.IsSatisfiedBy(o);
        }
    }
}
