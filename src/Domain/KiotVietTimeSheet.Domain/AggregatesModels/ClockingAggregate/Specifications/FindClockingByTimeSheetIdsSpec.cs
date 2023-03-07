using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByTimeSheetIdsSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByTimeSheetIdsSpec(List<long> timeSheetIds)
           : base(c => timeSheetIds.Contains(c.TimeSheetId))
        {
        }
    }
}
