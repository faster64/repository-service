using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetByCommissionCurrentBranch
{
    public class GetByCommissionCurrentBranchQuery : QueryBase<List<CommissionDto>>
    {
        public bool IncludeDeleted { get; set; }
        public bool IncludeInActive { get; set; }
        public List<long> IncludeCommissionIds { get; set; }

        public GetByCommissionCurrentBranchQuery(bool includeDeleted = false, bool includeInActive = false,
            List<long> includeCommissionIds = null)
        {
            IncludeDeleted = includeDeleted;
            IncludeInActive = includeInActive;
            IncludeCommissionIds = includeCommissionIds;
        }
    }
}
