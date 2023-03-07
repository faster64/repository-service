using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByUserIdSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByUserIdSpec(long? userId)
            : base(e => e.UserId == userId)
        {

        }
    }
}
