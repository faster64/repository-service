using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Specifications
{
    public class GetPayRateBySalaryPeriodSpec : ExpressionSpecification<PayRate>
    {
        public GetPayRateBySalaryPeriodSpec(PaySheetWorkingPeriodStatuses salaryPeriod)
            : base(e => e.SalaryPeriod == (byte)salaryPeriod)
        {
        }
    }
}
