using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindEmployeeBranchByEmployeeIdSpec : ExpressionSpecification<EmployeeBranch>
    {
        public FindEmployeeBranchByEmployeeIdSpec(long employeeId)
            : base(e => e.EmployeeId == employeeId)
        { }
    }
}
