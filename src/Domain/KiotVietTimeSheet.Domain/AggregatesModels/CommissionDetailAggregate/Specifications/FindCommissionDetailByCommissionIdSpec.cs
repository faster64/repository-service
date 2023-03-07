using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByCommissionIdSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByCommissionIdSpec(long commissionId)
            : base(e => e.CommissionId == commissionId)
        {
        }
    }
}
