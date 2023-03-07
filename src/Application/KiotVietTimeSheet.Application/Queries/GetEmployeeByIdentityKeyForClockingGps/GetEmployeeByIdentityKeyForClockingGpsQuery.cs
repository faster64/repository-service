using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByIdentityKeyForClockingGps
{
    [RequiredPermission(ClockingGpsPermission.Full)]
    public class GetEmployeeByIdentityKeyForClockingGpsQuery : QueryBase<EmployeeDto>
    {
        public string IdentityKeyClocking { get; set; }
        public GetEmployeeByIdentityKeyForClockingGpsQuery(

            string identityKeyClocking)
        {
            IdentityKeyClocking = identityKeyClocking;
        }
    }
}
