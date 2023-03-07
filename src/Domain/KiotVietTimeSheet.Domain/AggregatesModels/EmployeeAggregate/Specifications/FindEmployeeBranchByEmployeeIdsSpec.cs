using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindBranchByEmployeeIdsSpec : ExpressionSpecification<EmployeeBranch>
    {
        public FindBranchByEmployeeIdsSpec(List<long> employeeIds)
            : base(e => employeeIds.Contains(e.EmployeeId))
        { }

    }
}
