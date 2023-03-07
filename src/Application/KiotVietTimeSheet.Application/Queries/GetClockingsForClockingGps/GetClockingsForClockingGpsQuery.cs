using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetClockingsForClockingGps
{
    [RequiredPermission(ClockingGpsPermission.Full)]
    public class GetClockingsForClockingGpsQuery : QueryBase<ClockingGpsDto>
    {
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }

        public GetClockingsForClockingGpsQuery(int branchId, long employeeId)
        {
            BranchId = branchId;
            EmployeeId = employeeId;
        }
    }
}
