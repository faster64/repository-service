using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Specifications
{
    public class FindPayslipPenalizeByIsActiveSpec : ExpressionSpecification<PayslipPenalize>
    {
        public FindPayslipPenalizeByIsActiveSpec(bool isActive)
            : base(e => e.IsActive == isActive)
        { }
    }
}
