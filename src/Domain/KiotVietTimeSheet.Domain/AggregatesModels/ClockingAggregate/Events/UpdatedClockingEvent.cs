using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Events
{
    public class UpdatedClockingEvent : DomainEvent
    {
        public Clocking OldClocking { get; set; }
        public Clocking NewClocking { get; set; }
        public UpdatedClockingEvent(Clocking oldClocking, Clocking clocking)
        {
            NewClocking = clocking;
            OldClocking = oldClocking;
        }
    }
}
