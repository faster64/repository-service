using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.EventBus.Events.PayRateTemplateEvents
{
    public class DeletedPayRateTemplateIntegrationEvent : IntegrationEvent
    {
        public PayRateTemplate PayRateTemplate { get; set; }
        public bool IsGeneralSetting { get; set; }

        public DeletedPayRateTemplateIntegrationEvent(PayRateTemplate payRateTemplate, bool isGeneralSetting)
        {
            PayRateTemplate = payRateTemplate;
            IsGeneralSetting = isGeneralSetting;
        }

        [JsonConstructor]
        public DeletedPayRateTemplateIntegrationEvent()
        {

        }
    }
}
