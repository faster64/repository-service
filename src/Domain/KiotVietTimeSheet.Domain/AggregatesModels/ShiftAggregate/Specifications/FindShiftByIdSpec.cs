using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftByIdSpec : ExpressionSpecification<Shift>
    {
        public FindShiftByIdSpec(long id)
            : base(s => s.Id == id) { }
    }
}
