using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.QueryFilter;

namespace KiotVietTimeSheet.Application.AppQueries.QueryFilters
{
    public class PayslipClockingPenalizeByPayslipIdFilter : QueryFilterBase<Payslip>
    {
        public long? PayslipId { get; set; }
    }
}
