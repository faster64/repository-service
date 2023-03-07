using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetShiftByTimeSheetIdsSpec : ExpressionSpecification<TimeSheetShift>
    {
        public FindTimeSheetShiftByTimeSheetIdsSpec(List<long> timeSheetIds)
        : base(c => timeSheetIds.Contains(c.TimeSheetId))
        {
        }
    }
}
