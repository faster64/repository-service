using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Validations
{
    public class CreateOrUpdateBranchSettingValidator : BranchSettingValidator<BranchSetting>
    {
        public CreateOrUpdateBranchSettingValidator()
        {
            ValidateWorkingDays();
        }
    }
}
