using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftsByIds : ExpressionSpecification<Shift>
    {
        public FindShiftsByIds(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
