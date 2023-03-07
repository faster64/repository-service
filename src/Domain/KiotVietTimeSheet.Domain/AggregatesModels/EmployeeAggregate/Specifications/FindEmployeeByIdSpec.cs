using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByIdSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
