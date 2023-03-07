using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class UserAllPrivilegeDto
    {
        public Dictionary<string, IList<int>> PermissionMap { get; set; }
    }
}
