using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications
{
    public class GetAllowanceByIdsSpec : ExpressionSpecification<Allowance>
    {
        public GetAllowanceByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id))
        { }
    }
}
