using KiotVietTimeSheet.Application.Dto;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/settings/{TenantId}",
        "GET",
        Summary = "Lấy thông tin setting  của chi nhánh",
        Notes = "")
    ]
    public class GetSettingReq : IReturn<object>
    {
        public int TenantId { get; set; }
    }

    [Route("/settings/clearCacheTimeSheet/{TenantId}/{UserId}",
    "GET",
    Summary = "Xóa cache timesheet",
    Notes = "")]
    [Route("/settings/clearCacheTimeSheet/{TenantId}/{UserId}/{ClearPermit}",
        "GET",
        Summary = "Xóa cache timesheet",
        Notes = "")]
    public class ClearCacheTimeSheet : IReturn<object>
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public bool? ClearPermit { get; set; }
    }

    [Route("/settings/viewpermitbranch/{TenantId}/{UserId}", "GET")]
    public class ViewPermitBranch : IReturn<object>
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
    }

    [Route("/settings/googlemapsapikey", "GET")]
    public class GetGoogleMapApiKey : IReturn<object>
    {
    }
    #endregion

    #region POST classes
    [Route("/settings",
        "POST",
        Summary = "Tạo mới setting",
        Notes = "")
    ]
    public class CreateOrUpdateSettingReq : IReturn<object>
    {
        public SettingsDto Data { get; set; }
    }

    [Route("/settings/settings-for-clocking",
        "POST",
        Summary = "Tạo mới hoặc cập nhật settings cho phần chấm công",
        Notes = "")
    ]
    public class CreateOrUpdateClockingSettingReq : IReturn<object>
    {
        public SettingsDto Data { get; set; }
    }

    [Route("/settings/settings-for-timesheet",
        "POST",
        Summary = "Tạo mới hoặc cập nhật settings cho phần tính lương",
        Notes = "")
    ]
    public class CreateOrUpdateTimesheetSettingReq : IReturn<object>
    {
        public SettingsDto Data { get; set; }
    }

    [Route("/settings/settings-for-commission",
        "POST",
        Summary = "Tạo mới hoặc cập nhật settings cho phần hoa hồng",
        Notes = "")
    ]
    public class CreateOrUpdateCommissionSettingReq : IReturn<object>
    {
        public SettingsDto Data { get; set; }
    }

    [Route("/settings/updateUseClockingGps",
        "POST",
        Summary = "Cập nhật thiết lập chấm công Gps",
        Notes = "")
    ]
    public class UpdateSettingUseClockingGpsReq : IReturn<object>
    {
        public bool UseClockingGps { get; set; }
    }
    #endregion
}
