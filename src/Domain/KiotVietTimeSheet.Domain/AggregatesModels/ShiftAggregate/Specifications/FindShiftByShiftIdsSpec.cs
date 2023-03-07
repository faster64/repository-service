using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications
{
    public class FindShiftByShiftIdsSpec : ExpressionSpecification<Shift>
    {
        public FindShiftByShiftIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
