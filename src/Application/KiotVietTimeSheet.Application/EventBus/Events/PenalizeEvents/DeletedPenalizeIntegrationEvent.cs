using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents
{
    public class DeletedPenalizeIntegrationEvent : IntegrationEvent
    {
        public Penalize Penalize { get; set; }

        public DeletedPenalizeIntegrationEvent(Penalize penalize)
        {
            Penalize = penalize;
        }

        [JsonConstructor]
        public DeletedPenalizeIntegrationEvent()
        {

        }
    }
}
