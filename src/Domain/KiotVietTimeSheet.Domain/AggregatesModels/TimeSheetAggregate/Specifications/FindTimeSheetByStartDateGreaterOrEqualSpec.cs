using System;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByStartDateGreaterOrEqualSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByStartDateGreaterOrEqualSpec(DateTime from)
            : base(ts => ts.StartDate >= from.Date)
        {
        }
    }
}
