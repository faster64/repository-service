using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeById
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeByIdQuery : QueryBase<EmployeeDto>
    {
        public long Id { get; set; }

        public GetEmployeeByIdQuery(long id)
        {
            Id = id;
        }
    }
}
