using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Events
{
    public class UpdatedBranchSettingEvent : DomainEvent
    {
        public BranchSetting BranchSetting { get; private set; }
        public UpdatedBranchSettingEvent(BranchSetting branchSetting)
        {
            BranchSetting = branchSetting;
        }
    }
}
