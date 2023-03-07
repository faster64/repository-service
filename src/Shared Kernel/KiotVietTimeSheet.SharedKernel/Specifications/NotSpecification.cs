using System;
using System.Linq;
using System.Linq.Expressions;

namespace KiotVietTimeSheet.SharedKernel.Specifications
{
    public class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification;
        public NotSpecification(ISpecification<T> specification)
        {

            _specification = specification;
        }

        public override Expression<Func<T, bool>> GetExpression()
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(_specification.GetExpression().Body), _specification.GetExpression().Parameters.Single());
        }

        public override bool IsSatisfiedBy(T o)
        {
            return !_specification.IsSatisfiedBy(o);
        }
    }
}
