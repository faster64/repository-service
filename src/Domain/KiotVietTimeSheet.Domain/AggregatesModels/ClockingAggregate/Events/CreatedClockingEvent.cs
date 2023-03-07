using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class CreatedClockingEvent : DomainEvent
    {
        public Clocking Clocking { get; private set; }
        public CreatedClockingEvent(Clocking clocking)
        {
            Clocking = clocking;
        }
    }
}
