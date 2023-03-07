using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByIdsSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
