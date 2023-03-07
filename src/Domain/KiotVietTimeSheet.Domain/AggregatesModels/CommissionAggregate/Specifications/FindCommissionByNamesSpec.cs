using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications
{
    public class FindCommissionByNameSpec : ExpressionSpecification<Commission>
    {
        public FindCommissionByNameSpec(string name)
            : base(s => s.Name == name) { }
    }
}
