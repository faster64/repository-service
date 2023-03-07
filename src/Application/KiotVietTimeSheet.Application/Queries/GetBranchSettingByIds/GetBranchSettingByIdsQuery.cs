using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetBranchSettingByIds
{
    [RequiredPermission(TimeSheetPermission.BranchSetting_Read)]
    public class GetBranchSettingByIdsQuery : QueryBase<List<BranchSettingDto>>
    {
        public List<int> Ids { get; set; }

        public GetBranchSettingByIdsQuery(List<int> ids)
        {
            Ids = ids;
        }
    }
}
