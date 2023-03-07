using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingIsCancelOrNotCheckedInByIdsSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingIsCancelOrNotCheckedInByIdsSpec(List<long> ids)
            : base(e => ids.Contains(e.Id) && (e.ClockingStatus == (byte)ClockingStatuses.Void || e.ClockingStatus == (byte)ClockingStatuses.Created))
        { }
    }
}
