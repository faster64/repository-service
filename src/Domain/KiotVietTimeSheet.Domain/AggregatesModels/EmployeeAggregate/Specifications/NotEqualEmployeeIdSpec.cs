using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class NotEqualEmployeeIdSpec : ExpressionSpecification<Employee>
    {
        public NotEqualEmployeeIdSpec(long id)
            : base(e => e.Id != id)
        { }
    }
}
