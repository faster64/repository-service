using System;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByEndTimeGreaterThanOrEqualSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByEndTimeGreaterThanOrEqualSpec(DateTime endTime)
            : base(c => c.EndTime >= endTime)
        {
        }
    }
}
