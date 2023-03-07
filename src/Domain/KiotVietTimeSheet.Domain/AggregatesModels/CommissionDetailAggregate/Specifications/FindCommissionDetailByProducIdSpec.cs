using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByProducIdSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByProducIdSpec(long productId)
            : base(e => e.ObjectId == productId && e.Type == (byte)CommissionDetailType.Product)
        {
        }
    }
}
