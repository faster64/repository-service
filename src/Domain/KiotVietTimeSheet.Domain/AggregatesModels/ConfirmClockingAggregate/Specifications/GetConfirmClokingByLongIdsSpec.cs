using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Specifications
{
    public class GetConfirmClokingByLongIdsSpec : ExpressionSpecification<ConfirmClocking>
    {
        public GetConfirmClokingByLongIdsSpec(List<long> ids)
          : base(entity => ids.Contains(entity.Id))
        {

        }
    }
}
