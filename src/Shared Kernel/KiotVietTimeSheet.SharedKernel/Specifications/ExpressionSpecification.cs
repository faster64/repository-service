using System;
using System.Linq.Expressions;

namespace KiotVietTimeSheet.SharedKernel.Specifications
{
    public class ExpressionSpecification<T> : Specification<T>
    {
        private Func<T, bool> _compiledExpression;
        private Expression<Func<T, bool>> Expression { get; }
        private Func<T, bool> CompiledExpression => _compiledExpression ?? (_compiledExpression = Expression.Compile());

        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            return Expression;
        }

        public override bool IsSatisfiedBy(T o)
        {
            return CompiledExpression(o);
        }
    }
}
