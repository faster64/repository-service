using System;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByStartDateLessThanOrEqualSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByStartDateLessThanOrEqualSpec(DateTime dateTime)
            : base(c => c.StartDate <= dateTime)
        {
        }
    }
}
