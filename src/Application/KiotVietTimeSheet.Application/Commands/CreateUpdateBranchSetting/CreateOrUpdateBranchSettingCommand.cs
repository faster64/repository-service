using System;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateUpdateBranchSetting
{
    [RequiresAnyPermission(TimeSheetPermission.BranchSetting_Create, TimeSheetPermission.BranchSetting_Update)]
    public class CreateOrUpdateBranchSettingCommand : BaseCommand<BranchSettingDto>
    {
        public BranchSettingDto BranchSettingDto { get; set; }
        public bool IsAddMore { get; set; }
        public bool IsRemove { get; set; }
        public DateTime ApplyFrom { get; set; }

        public CreateOrUpdateBranchSettingCommand(BranchSettingDto branchSettingDto, bool isAddMore, bool isRemove, DateTime applyFrom)
        {
            BranchSettingDto = branchSettingDto;
            IsAddMore = isAddMore;
            IsRemove = isRemove;
            ApplyFrom = applyFrom;
        }
    }
}
