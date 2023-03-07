using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class UserPrivilegeDto
    {
        public long UserId { get; set; }
        public int? BranchId { get; set; }

        public long? RoleId { get; set; }
        public IDictionary<string, bool> Data { get; set; }
        public string CompareUserName { get; set; }
        public string CompareGivenName { get; set; }
    }
}
