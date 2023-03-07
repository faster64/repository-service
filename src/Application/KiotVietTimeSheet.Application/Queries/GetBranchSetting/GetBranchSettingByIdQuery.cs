using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetBranchSetting
{
    [RequiredPermission(TimeSheetPermission.BranchSetting_Read)]
    public class GetBranchSettingByIdQuery : QueryBase<BranchSettingDto>
    {
        public int Id { get; set; }

        public GetBranchSettingByIdQuery(int id)
        {
            Id = id;
        }
    }
}
