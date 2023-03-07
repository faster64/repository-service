using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Dto
{
    public class CommissionDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<int> BranchIds { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAllBranch { get; set; }
        public List<CommissionBranchDto> CommissionBranches { get; set; }
        public List<CommissionDetailDto> CommissionDetails { get; set; }
    }
}
