using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Application.EventBus.Events.ClockingEvents
{
    public class UpdateClockingTimeIntegrationEvent : IntegrationEvent
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public long ShiftId { get; set; }
        public string ShiftName { get; set; }
        public long From { get; set; }
        public long To { get; set; }

        public UpdateClockingTimeIntegrationEvent(int tenantId, int branchId, long shiftId, string shiftName, long from, long to)
        {
            TenantId = tenantId;
            BranchId = branchId;
            ShiftId = shiftId;
            ShiftName = shiftName;
            From = from;
            To = to;
        }
    }
}