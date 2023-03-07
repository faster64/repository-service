using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByNameOrCodeSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByNameOrCodeSpec(string keyword)
            : base(e => e.Name.Contains(keyword) || e.Code.Contains(keyword))
        {

        }
    }
}
