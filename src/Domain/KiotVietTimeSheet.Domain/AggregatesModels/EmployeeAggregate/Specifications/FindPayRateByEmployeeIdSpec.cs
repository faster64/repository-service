using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications
{
    public class FindPayRateByEmployeeIdSpec : ExpressionSpecification<PayRate>
    {
        public FindPayRateByEmployeeIdSpec(long employeeId)
           : base(pr => pr.EmployeeId == employeeId)
        { }
    }
}
