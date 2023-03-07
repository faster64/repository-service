using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByCurrentUserId
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeByCurrentUserIdQuery : QueryBase<EmployeeDto>
    {
        public bool Reference { get; set; }
        public bool IncludeSoftDelete { get; set; }
        public GetEmployeeByCurrentUserIdQuery(bool reference, bool includeSoftDelete)
        {
            Reference = reference;
            IncludeSoftDelete = includeSoftDelete;
        }
    }
}
