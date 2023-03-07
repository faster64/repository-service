using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications
{
    public class FindDeductionByIdSpec : ExpressionSpecification<Deduction>
    {
        public FindDeductionByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
