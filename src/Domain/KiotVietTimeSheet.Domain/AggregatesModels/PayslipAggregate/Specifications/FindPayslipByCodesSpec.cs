using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByCodesSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByCodesSpec(List<string> codes)
            : base(e => codes.Contains(e.Code.Trim().ToLower()))
        { }
    }
}
