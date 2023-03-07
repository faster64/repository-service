using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents
{
    public class UpdatedPayRateTemplateIntegrationEvent : IntegrationEvent
    {
        public PayRateTemplate OldPayRateTemplate { get; set; }
        public PayRateTemplate NewPayRateTemplate { get; set; }
        public bool UpdatePayRate { get; set; }
        public bool IsGeneralSetting { get; set; }

        public UpdatedPayRateTemplateIntegrationEvent(UpdatedPayRateTemplateEvent @event)
        {
            OldPayRateTemplate = @event.OldPayRateTemplate;
            NewPayRateTemplate = @event.NewPayRateTemplate;
            UpdatePayRate = @event.UpdatePayRate;
            IsGeneralSetting = @event.IsGeneralSetting;
        }

        [JsonConstructor]
        public UpdatedPayRateTemplateIntegrationEvent()
        {

        }
    }
}
