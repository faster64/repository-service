using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetBySalaryPeriod : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetBySalaryPeriod(byte salaryPeriod)
            : base(p => p.SalaryPeriod == salaryPeriod)
        { }
    }
}
