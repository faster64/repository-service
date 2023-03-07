using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetShiftMultipleBranchOrderByFromAndTo
{
    public sealed class GetShiftMultipleBranchOrderByFromAndToQuery : QueryBase<List<ShiftDto>>
    {
        public List<int> BranchIds { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long> IncludeShiftIds { get; set; }
        public bool IncludeDeleted { get; set; }

        public GetShiftMultipleBranchOrderByFromAndToQuery(List<int> branchIds, List<long> shiftIds, List<long> includeShiftIds, bool includeDeleted)
        {
            BranchIds = branchIds;
            ShiftIds = shiftIds;
            IncludeShiftIds = includeShiftIds;
            IncludeDeleted = includeDeleted;
        }
    }
}
