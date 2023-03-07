using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region POST methods
    [Route("/branch-setting",
         "POST",
         Summary = "Thêm mới/Cập nhật settings của chi nhánh",
         Notes = "")
     ]
    public class CreateOrUpdateBranchSettingReq : IReturn<object>
    {
        public BranchSettingDto BranchSetting { get; set; }
        public bool IsAddMore { get; set; }
        public bool IsRemove { get; set; }
        public DateTime ApplyFrom { get; set; }
    }

    [Route("/branch-setting/check-can-update-branch-setting",
        "POST",
        Summary = "Kiểm tra có thể thay đổi hay không",
        Notes = "")
    ]
    public class CheckCanUpdateBranchSettingReq : IReturn<object>
    {
        public BranchSettingDto BranchSetting { get; set; }
        public bool IsAddMore { get; set; }
        public bool IsRemove { get; set; }
        public DateTime ApplyFrom { get; set; }
    }
    #endregion

    #region GET methods
    [Route("/branch-setting/{BranchId}",
         "GET",
         Summary = "Lấy thông tin setting timesheet của chi nhánh",
         Notes = "")
     ]
    public class GetBranchSettingReq : IReturn<object>
    {
        public int BranchId { get; set; }
    }

    [Route("/branch-setting/get-branch-setting-by-branchids",
        "GET",
        Summary = "Lấy thông tin setting timesheet của chi nhánh",
        Notes = "")
    ]
    public class GetBranchSettingByBranchIdsReq : IReturn<object>
    {
        public List<int> BranchIds { get; set; }
    }
    #endregion
}
