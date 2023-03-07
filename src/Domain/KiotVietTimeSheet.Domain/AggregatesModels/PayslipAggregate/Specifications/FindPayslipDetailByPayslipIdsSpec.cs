using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipDetailByPayslipIdsSpec : ExpressionSpecification<PayslipDetail>
    {
        public FindPayslipDetailByPayslipIdsSpec(List<long> payslipIds)
            : base(p => payslipIds.Contains(p.PayslipId)) { }
    }
}
