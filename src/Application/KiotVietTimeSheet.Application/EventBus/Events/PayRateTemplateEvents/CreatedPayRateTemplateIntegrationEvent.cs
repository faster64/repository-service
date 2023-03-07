using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents
{
    public class CreatedPayRateTemplateIntegrationEvent : IntegrationEvent
    {
        public PayRateTemplate PayRateTemplate { get; set; }
        public bool IsGeneralSetting { get; set; }

        [JsonConstructor]
        public CreatedPayRateTemplateIntegrationEvent()
        {

        }

        public CreatedPayRateTemplateIntegrationEvent(CreatedPayRateTemplateEvent @event)
        {
            PayRateTemplate = @event.PayRateTemplate;
            IsGeneralSetting = @event.IsGeneralSetting;
        }
    }
}
