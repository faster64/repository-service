using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByTimeRange : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByTimeRange(DateTime startTime, DateTime endTime)
            : base(p => (p.StartTime >= startTime && p.StartTime < endTime)
                                || (p.EndTime >= startTime && p.EndTime < endTime)
                                || (p.StartTime <= startTime && p.EndTime >= endTime)
                )
        { }
    }
}
