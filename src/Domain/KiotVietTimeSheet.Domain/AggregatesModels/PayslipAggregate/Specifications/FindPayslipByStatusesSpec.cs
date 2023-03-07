using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByStatusesSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByStatusesSpec(List<byte> statuses)
            : base(p => statuses.Contains(p.PayslipStatus))
        { }
    }
}
