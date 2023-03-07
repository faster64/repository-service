using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByEmployeeIds : ExpressionSpecification<Payslip>
    {
        public FindPayslipByEmployeeIds(List<long> employeeIds)
          : base(e => employeeIds.Contains(e.EmployeeId))
        { }
    }
}
