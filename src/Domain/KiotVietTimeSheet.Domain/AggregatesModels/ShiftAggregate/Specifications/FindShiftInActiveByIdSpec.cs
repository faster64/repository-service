using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftInActiveByIdSpec : ExpressionSpecification<Shift>
    {
        public FindShiftInActiveByIdSpec(long id)
            : base(s => s.Id == id && !s.IsActive)
        { }
    }
}
