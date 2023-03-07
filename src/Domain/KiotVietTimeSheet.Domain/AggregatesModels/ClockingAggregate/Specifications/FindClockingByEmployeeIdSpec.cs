using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByEmployeeIdSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByEmployeeIdSpec(long employeeId)
            : base(c => c.EmployeeId == employeeId)
        {
        }
    }
}
