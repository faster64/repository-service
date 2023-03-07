using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByIdsSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByIdsSpec(List<long> ids)
        : base(c => ids.Contains(c.Id))
        {
        }
    }
}
