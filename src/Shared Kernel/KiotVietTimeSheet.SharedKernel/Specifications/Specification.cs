using System;
using System.Linq.Expressions;

namespace KiotVietTimeSheet.SharedKernel.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> GetExpression();

        public abstract bool IsSatisfiedBy(T o);

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public ISpecification<T> Not(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, new NotSpecification<T>(specification));
        }
    }
}
