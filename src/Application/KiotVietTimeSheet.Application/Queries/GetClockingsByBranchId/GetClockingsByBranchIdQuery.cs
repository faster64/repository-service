using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsByBranchId
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetClockingsByBranchIdQuery : QueryBase<List<ClockingDto>>
    {
        public int BranchId { get; set; }

        public GetClockingsByBranchIdQuery(int branchId)
        {
            BranchId = branchId;
        }
    }
}
