using System;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByStartDateLessThanSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByStartDateLessThanSpec(DateTime dateTime)
            : base(c => c.StartDate < dateTime)
        {
        }
    }
}
