using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionByIdSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionByIdSpec(long id)
            : base(s => s.Id == id) { }
    }
}
