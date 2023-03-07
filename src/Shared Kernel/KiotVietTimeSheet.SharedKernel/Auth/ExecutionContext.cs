namespace KiotVietTimeSheet.SharedKernel.Auth
{
    public class ExecutionContext
    {
        public int TenantId { get; set; }
        public string TenantCode { get; set; }
        public int BranchId { get; set; }
        public SessionUser User { get; set; }
        public string JwtToken { get; set; }
        public string Language { get; set; }
        public string HttpMethod { get; set; }
        public string TimeSheetConnection { get; set; }
        public bool IsBackgroundService { get; set; }
        public bool IsAuthorized()
        {
            return TenantId > 0 && !string.IsNullOrEmpty(TenantCode) && BranchId > 0 && User != null && User.Id > 0;
        }
    }
}
