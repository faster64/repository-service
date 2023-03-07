using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class BranchSettingDto
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public List<byte> WorkingDays { get; set; }
        public int TenantId { get; set; }
    }
}
