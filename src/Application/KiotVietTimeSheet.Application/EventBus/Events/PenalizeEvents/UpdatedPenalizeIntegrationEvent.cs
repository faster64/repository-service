using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PenalizeEvents
{
    public class UpdatedPenalizeIntegrationEvent : IntegrationEvent
    {
        public Penalize PenalizeNew { get; set; }
        public Penalize PenalizeOld { get; set; }

        public UpdatedPenalizeIntegrationEvent(Penalize penalizeNew, Penalize penalizeOld)
        {
            PenalizeNew = penalizeNew;
            PenalizeOld = penalizeOld;
        }

        [JsonConstructor]
        public UpdatedPenalizeIntegrationEvent()
        {

        }
    }
}
