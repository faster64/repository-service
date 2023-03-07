using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByEmployeeIdSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByEmployeeIdSpec(long employeeId)
            : base(c => c.EmployeeId == employeeId)
        {
        }
    }
}
