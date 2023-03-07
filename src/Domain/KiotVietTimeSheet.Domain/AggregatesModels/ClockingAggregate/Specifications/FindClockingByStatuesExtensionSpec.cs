using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByStatuesExtensionSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByStatuesExtensionSpec(List<byte> statusExtension)
            : base(c => (statusExtension.Contains((byte)ClockingStatusesExtension.Created) &&
                         c.ClockingStatus == (byte)ClockingStatuses.Created) ||
                        (statusExtension.Contains((byte)ClockingStatusesExtension.CheckInNoCheckOut) &&
                         c.ClockingStatus == (byte)ClockingStatuses.CheckedIn) ||
                        (statusExtension.Contains((byte)ClockingStatusesExtension.CheckOutNoCheckIn) &&
                         c.ClockingStatus == (byte)ClockingStatuses.CheckedOut && c.CheckInDate == null) ||
                        (statusExtension.Contains((byte)ClockingStatusesExtension.CheckInCheckOut) &&
                         c.ClockingStatus == (byte)ClockingStatuses.CheckedOut && c.CheckInDate != null) ||
                        (statusExtension.Contains((byte)ClockingStatusesExtension.WorkOff) &&
                         c.ClockingStatus == (byte)ClockingStatuses.WorkOff))
        { }
    }
}
