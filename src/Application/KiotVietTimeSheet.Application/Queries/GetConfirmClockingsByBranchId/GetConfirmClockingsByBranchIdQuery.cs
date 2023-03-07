using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetConfirmClockingsByBranchId
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetConfirmClockingsByBranchIdQuery : QueryBase<List<ConfirmClockingDto>>
    {
        public int BranchId { get; set; }

        public GetConfirmClockingsByBranchIdQuery(int branchId)
        {
            BranchId = branchId;
        }
    }
}
