using KiotVietTimeSheet.SharedKernel.Auth;
using ServiceStack;
using KiotVietTimeSheet.Infrastructure.KiotVietApiClient.Dtos;

namespace KiotVietTimeSheet.Infrastructure.Securities.KvAuth
{
    public class KVSession : AuthUserSession
    {
        public SessionUser CurrentUser { get; set; }
        public string KvSessionId { get; set; }
        public int CurrentRetailerId { get; set; }
        public int CurrentIndustryId { get; set; }
        public int GroupId { get; set; }
        public string CurrentRetailerCode { get; set; }
        public string CurrentLang { get; set; }
        public int CurrentBranchId { get; set; }
        public string HttpMethod { get; set; }
        public FnbBaseInformation SessionInfo { get; set; }
    }
}
