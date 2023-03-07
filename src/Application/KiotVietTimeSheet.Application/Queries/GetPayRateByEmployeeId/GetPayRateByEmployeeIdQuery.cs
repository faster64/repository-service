using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeId
{
    [RequiredPermission(TimeSheetPermission.PayRate_Read)]
    public class GetPayRateByEmployeeIdQuery : QueryBase<PayRateDto>
    {
        public long EmployeeId { get; set; }

        public GetPayRateByEmployeeIdQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
