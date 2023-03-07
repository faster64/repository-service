using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdatePayrateTemplate
{
    public class UpdatePayrateTemplateCommand : BaseCommand<PayRateFormDto>
    {
        public long Id { get; set; }
        public PayRateFormDto PayRateTemplate { get; set; }
        public bool UpdatePayRate { get; set; }
        public bool IsGeneralSetting { get; set; }

        public UpdatePayrateTemplateCommand(PayRateFormDto payRateTemplate, long id, bool updatePayRate, bool isGeneralSetting)
        {
            PayRateTemplate = payRateTemplate;
            Id = id;
            UpdatePayRate = updatePayRate;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
