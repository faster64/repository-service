using System;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByStartTimeLessThanOrEqualSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByStartTimeLessThanOrEqualSpec(DateTime startTime)
            : base(c => c.StartTime <= startTime)
        {
        }
    }
}
