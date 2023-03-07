using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByEmployeeId : ExpressionSpecification<Payslip>
    {
        public FindPayslipByEmployeeId(long employeeId)
          : base(e => e.EmployeeId == employeeId)
        { }
    }
}
