using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByEmployeeIdsSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByEmployeeIdsSpec(List<long> employeeIds)
            : base(c => employeeIds.Contains(c.EmployeeId))
        {
        }
    }
}
