using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class GetPayslipsByPaysheetIds : ExpressionSpecification<Payslip>
    {
        public GetPayslipsByPaysheetIds(ICollection<long> paysheetIds)
            : base(e => paysheetIds.Contains(e.PaysheetId))
        { }
    }
}
