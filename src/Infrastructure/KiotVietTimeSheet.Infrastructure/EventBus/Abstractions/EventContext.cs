using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Auth;

namespace KiotVietTimeSheet.Infrastructure.EventBus.Abstractions
{
    public class EventContext
    {
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public int GroupId { get; set; }
        public long UserId { get; set; }
        public string BearToken { get; set; }
        public string RetailerCode { get; set; }
        public SessionUser User { get; set; }
        public IList<int> AuthorizedBranchIds { get; set; }
        public string TimeSheetConnection { get; set; }
        public IList<string> Permissions { get; set; }
        public IDictionary<string, IList<string>> AtLeastNecessaryPermissionMap { get; set; }
        public string Language { get; set; }
    }
}
