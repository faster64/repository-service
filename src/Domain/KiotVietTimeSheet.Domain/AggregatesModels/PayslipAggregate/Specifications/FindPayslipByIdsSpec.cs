using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipByIdsSpec : ExpressionSpecification<Payslip>
    {
        public FindPayslipByIdsSpec(ICollection<long> ids)
            : base(p => ids.Contains(p.Id))
        { }
    }
}
