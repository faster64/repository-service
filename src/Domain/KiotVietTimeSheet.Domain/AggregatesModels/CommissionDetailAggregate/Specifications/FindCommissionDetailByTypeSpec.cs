using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByTypeSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByTypeSpec(byte type)
            : base(c => c.Type == type)
        {
        }
    }
}
