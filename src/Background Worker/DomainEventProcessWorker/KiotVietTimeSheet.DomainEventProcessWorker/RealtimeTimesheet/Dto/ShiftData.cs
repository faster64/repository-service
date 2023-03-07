using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.DomainEventProcessWorker.RealtimeTimesheet.Dto
{
    public class ShiftData : IntegrationEvent
    {
        public int RetailerId { get; private set; }
        public Shift Shift { get; set; }
        public ShiftData(int retailerId, Shift shift)
        {
            RetailerId = retailerId;
            Shift = shift;
        }
    }
}
