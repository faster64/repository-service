using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Events
{
    public class CreatedBranchSettingEvent : DomainEvent
    {
        public BranchSetting BranchSetting { get; private set; }
        public CreatedBranchSettingEvent(BranchSetting branchSetting)
        {
            BranchSetting = branchSetting;
        }
    }
}
