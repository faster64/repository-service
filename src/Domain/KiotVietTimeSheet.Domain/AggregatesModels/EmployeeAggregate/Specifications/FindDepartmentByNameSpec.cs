using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindDepartmentByNameSpec : ExpressionSpecification<Department>
    {
        public FindDepartmentByNameSpec(string name)
            : base(e => e.Name.Equals(name.ToPerfectString()))
        { }
    }
}
