using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents
{
    public class CreatedPenalizeIntegrationEvent : IntegrationEvent
    {
        public Penalize Penalize { get; set; }

        public CreatedPenalizeIntegrationEvent(Penalize penalize)
        {
            Penalize = penalize;
        }

        [JsonConstructor]
        public CreatedPenalizeIntegrationEvent()
        {

        }
    }
}
