using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Events
{
    public class UpdatedPayRateTemplateEvent : DomainEvent
    {
        public PayRateTemplate OldPayRateTemplate { get; set; }
        public PayRateTemplate NewPayRateTemplate { get; set; }
        public bool UpdatePayRate { get; set; }
        public bool IsGeneralSetting { get; set; }

        public UpdatedPayRateTemplateEvent(PayRateTemplate oldPayRateTemplate, PayRateTemplate newPayRateTemplate, bool updatePayRate, bool isGeneralSetting)
        {
            OldPayRateTemplate = oldPayRateTemplate;
            NewPayRateTemplate = newPayRateTemplate;
            UpdatePayRate = updatePayRate;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
