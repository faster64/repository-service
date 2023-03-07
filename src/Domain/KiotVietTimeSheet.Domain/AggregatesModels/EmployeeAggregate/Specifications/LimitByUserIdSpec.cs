using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class LimitByUserIdSpec : ExpressionSpecification<Employee>
    {
        public LimitByUserIdSpec(long? userId)
           : base(entity => entity.UserId == userId)
        {

        }
    }
}
