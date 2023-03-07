using System;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications
{
    public class FindTimeSheetByEndDateGreaterThanSpec : ExpressionSpecification<TimeSheet>
    {
        public FindTimeSheetByEndDateGreaterThanSpec(DateTime dateTime)
            : base(c => c.EndDate > dateTime)
        {
        }
    }
}
