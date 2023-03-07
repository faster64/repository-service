using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Specifications
{
    public class FindCommissionDetailByIdSpec : ExpressionSpecification<CommissionDetail>
    {
        public FindCommissionDetailByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
