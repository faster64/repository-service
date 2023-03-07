using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.CommissionEvents
{
    public class CreatedCommissionIntegrationEvent : IntegrationEvent
    {
        public Commission Commission { get; set; }
        public List<int> BranchIds { get; set; }

        [JsonConstructor]
        public CreatedCommissionIntegrationEvent()
        {

        }

        public CreatedCommissionIntegrationEvent(CreatedCommissionEvent @event)
        {
            Commission = @event.Commission;
            BranchIds = @event.BranchIds;
        }
    }
}
