using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications
{
    public class FindPenalizeByIdSpec : ExpressionSpecification<Penalize>
    {
        public FindPenalizeByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
