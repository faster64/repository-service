using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Queries.GetShiftByBranchId
{
    public sealed class GetShiftByBranchIdQuery : QueryBase<List<ShiftDto>>
    {
        public int BranchId  { get; set; }

        public GetShiftByBranchIdQuery(int branchId)
        {
            BranchId = branchId;
        }
    }
}
