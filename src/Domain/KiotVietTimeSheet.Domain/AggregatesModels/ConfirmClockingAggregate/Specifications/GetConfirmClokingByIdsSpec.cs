using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Specifications
{
    public class GetConfirmClokingByIdsSpec : ExpressionSpecification<ConfirmClocking>
    {
        public GetConfirmClokingByIdsSpec(ICollection<long> ids) : base(x=>ids.Contains(x.Id))
        {
        }
    }
}
