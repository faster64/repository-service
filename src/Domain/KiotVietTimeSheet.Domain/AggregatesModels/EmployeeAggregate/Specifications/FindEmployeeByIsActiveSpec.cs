using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByIsActiveSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByIsActiveSpec(bool isActive)
            : base(e => e.IsActive == isActive) { }
    }
}
