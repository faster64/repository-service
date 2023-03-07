using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetAndCheckTwoFaPin
{
    [RequiredPermission(ClockingGpsPermission.Full)]
    public class CheckTwoFaPinQuery : QueryBase<bool>
    {
        public long EmployeeId { get; set; }
        public string Pin { get; set; }

        public CheckTwoFaPinQuery(long employeeId, string pin)
        {
            EmployeeId = employeeId;
            Pin = pin;
        }
    }
}
