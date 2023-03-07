using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetOversionsByIdsSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetOversionsByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
