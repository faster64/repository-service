using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents
{
    public class DeletedCommissionIntegrationEvent : IntegrationEvent
    {
        public Commission Commission { get; set; }

        [JsonConstructor]
        public DeletedCommissionIntegrationEvent()
        {

        }

        public DeletedCommissionIntegrationEvent(DeletedCommissionEvent @event)
        {
            Commission = @event.Commission;
        }
    }
}
