using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftByNameSpec : ExpressionSpecification<Shift>
    {
        public FindShiftByNameSpec(string name)
            : base(s => s.Name.Trim().ToLower().Equals(name.Trim().ToLower())) { }
    }
}
