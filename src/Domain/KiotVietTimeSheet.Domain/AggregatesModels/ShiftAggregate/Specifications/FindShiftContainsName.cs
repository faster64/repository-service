using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftContainsName : ExpressionSpecification<Shift>
    {
        public FindShiftContainsName(string name)
            : base(s => s.Name.Trim().ToLower().Contains(name.Trim().ToLower())) { }
    }
}
