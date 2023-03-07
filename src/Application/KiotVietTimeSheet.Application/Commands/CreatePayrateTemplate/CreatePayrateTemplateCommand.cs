using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreatePayrateTemplate
{
    [Auth.Common.RequiredPermission(TimeSheetPermission.GeneralSettingPayrateTemplate_Create, TimeSheetPermission.GeneralSettingPayrateTemplate_Read)]
    public class CreatePayrateTemplateCommand : BaseCommand<PayRateFormDto>
    {
        public PayRateFormDto PayRateTemplate { get; set; }
        public int BranchId { get; set; }
        public bool IsGeneralSetting { get; set; }

        public CreatePayrateTemplateCommand(PayRateFormDto payRateTemplate, int branchId, bool isGeneralSetting)
        {
            PayRateTemplate = payRateTemplate;
            BranchId = branchId;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
