using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByTimeSheetIdsSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByTimeSheetIdsSpec(List<long> timeSheetIds)
            : base(c => timeSheetIds.Contains(c.Id))
        {
        }
    }
}
