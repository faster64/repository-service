using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications
{
    public class FindDeductionByNameSpec : ExpressionSpecification<Deduction>
    {
        public FindDeductionByNameSpec(string name)
            : base(e => e.Name.Equals(name.ToPerfectString()))
        { }
    }
}
