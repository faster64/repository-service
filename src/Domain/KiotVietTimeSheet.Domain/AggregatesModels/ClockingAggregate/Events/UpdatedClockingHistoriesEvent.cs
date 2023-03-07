using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class UpdatedClockingHistoriesEvent : DomainEvent
    {
        public List<ClockingHistory> ClockingHistories { get; set; }
    }
}
