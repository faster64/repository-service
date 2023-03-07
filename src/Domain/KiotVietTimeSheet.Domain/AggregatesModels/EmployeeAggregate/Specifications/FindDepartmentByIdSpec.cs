using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindDepartmentByIdSpec : ExpressionSpecification<Department>
    {
        public FindDepartmentByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
