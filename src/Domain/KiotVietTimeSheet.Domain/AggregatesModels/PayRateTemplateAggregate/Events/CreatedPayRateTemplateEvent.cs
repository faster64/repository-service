using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events
{
    public class CreatedPayRateTemplateEvent : DomainEvent
    {
        public PayRateTemplate PayRateTemplate { get; set; }
        public bool IsGeneralSetting { get; set; }

        public CreatedPayRateTemplateEvent(PayRateTemplate payRateTemplate, bool isGeneralSetting)
        {
            PayRateTemplate = payRateTemplate;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
