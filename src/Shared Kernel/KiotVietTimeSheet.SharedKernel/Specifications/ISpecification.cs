using System;
using System.Linq.Expressions;

namespace KiotVietTimeSheet.SharedKernel.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> GetExpression();
        bool IsSatisfiedBy(T o);
        ISpecification<T> And(ISpecification<T> specification);
        ISpecification<T> Or(ISpecification<T> specification);
        ISpecification<T> Not(ISpecification<T> specification);
    }
}
