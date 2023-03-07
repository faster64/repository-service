using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeByBranchIdSpec : ExpressionSpecification<Employee>
    {
        public FindEmployeeByBranchIdSpec(int branchId)
            : base(e => e.BranchId == branchId)
        { }
    }
}
