using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Commands.GetBranchesWhenHaveAnyClocking
{
    [RequiredPermission(TimeSheetPermission.Clocking_Delete)]
    public class GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommand : BaseCommand<PagingDataSource<BranchDto>>
    {
        public long EmployeeId { get; set; }
        public List<int> BranchCancelIds { get; set; }

        public GetBranchesWhenHaveAnyClockingAndClockingDeletePermissionCommand(long employeeId, List<int> branchCancelIds)
        {
            EmployeeId = employeeId;
            BranchCancelIds = branchCancelIds;
        }
    }
}
