using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class GetEmployeesByUserIds : ExpressionSpecification<Employee>
    {
        public GetEmployeesByUserIds(List<long> employeeIds)
            : base(e => employeeIds.Contains(e.UserId ?? 0))
        { }
    }
}
