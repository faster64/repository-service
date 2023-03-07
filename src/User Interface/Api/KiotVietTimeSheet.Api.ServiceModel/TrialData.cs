using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region POST methods
    [Route("/trialdata",
        "POST",
        Summary = "Tạo dữ liệu dùng thử",
        Notes = "")
    ]
    public class CreateBookingTrialDataReq : IReturn<object>
    {
        public int TrialType { get; set; }
        public int BranchId { get; set; }
        public long UserId1 { get; set; }
        public long UserId2 { get; set; }
        public long UserIdAdmin { get; set; }
        public string TenantCode { get; set; }
        public int TenantId { get; set; }
        public int GroupId { get; set; }
    }
    #endregion

    #region DELETE methods
    [Route("/delete-trialdata",
        "POST",
        Summary = "Xóa dữ liệu dùng thử",
        Notes = "")
    ]
    public class DeleteBookingTrialDataReq : IReturn<object>
    {
        public int TrialType { get; set; }
        public int BranchId { get; set; }
        public long UserIdAdmin { get; set; }
        public string TenantCode { get; set; }
        public int TenantId { get; set; }
        public int GroupId { get; set; }
    }

    #endregion
}
