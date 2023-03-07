using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications
{
    public class FindPenalizeByNameSpec : ExpressionSpecification<Penalize>
    {
        public FindPenalizeByNameSpec(string name)
            : base(e => e.Name.Equals(name.ToPerfectString()))
        { }
    }
}
