using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftActiveSpec : ExpressionSpecification<Shift>
    {
        public FindShiftActiveSpec()
            : base(s => s.IsActive)
        { }
    }
}
