using System;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByEndDateLessThanOrEqualSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByEndDateLessThanOrEqualSpec(DateTime to)
            : base(t => t.EndDate < to.Date.AddDays(1)) { }
    }
}
