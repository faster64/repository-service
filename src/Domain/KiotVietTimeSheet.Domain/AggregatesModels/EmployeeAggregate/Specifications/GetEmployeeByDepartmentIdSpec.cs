using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class GetEmployeeByDepartmentIdSpec : ExpressionSpecification<Employee>
    {
        public GetEmployeeByDepartmentIdSpec(long departmentId)
            : base(e => e.DepartmentId == departmentId)
        { }
    }
}
