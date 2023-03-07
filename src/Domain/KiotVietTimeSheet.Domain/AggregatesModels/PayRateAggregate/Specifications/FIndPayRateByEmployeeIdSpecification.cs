using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications
{
    public class FindPayRateByEmployeeIdSpecification : ExpressionSpecification<PayRate>
    {
        public FindPayRateByEmployeeIdSpecification(long employeeId)
            : base(e => e.EmployeeId == employeeId)
        { }
    }
}
