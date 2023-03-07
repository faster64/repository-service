using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipPenalizeByPayslipIds : ExpressionSpecification<PayslipPenalize>
    {
        public FindPayslipPenalizeByPayslipIds(ICollection<long> payslipIds)
            : base(e => payslipIds.Contains(e.PayslipId))
        { }
    }
}
