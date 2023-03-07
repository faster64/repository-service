using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipClockingPenalizeByPayslipIds : ExpressionSpecification<PayslipClockingPenalize>
    {
        public FindPayslipClockingPenalizeByPayslipIds(ICollection<long> payslipIds)
            : base(e => payslipIds.Contains(e.PayslipId))
        { }
    }
}
