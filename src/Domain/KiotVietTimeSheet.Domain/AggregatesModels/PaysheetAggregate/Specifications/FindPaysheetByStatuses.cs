using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByStatuses : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByStatuses(List<byte> paysheetStatuses)
            : base(p => paysheetStatuses.Contains(p.PaysheetStatus))
        { }
    }
}
