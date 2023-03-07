using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetDepartmentById
{
    [RequiredPermission(TimeSheetPermission.Department_Read)]
    public class GetDepartmentByIdQuery : QueryBase<DepartmentDto>
    {
        public long Id { get; set; }

        public GetDepartmentByIdQuery(long id)
        {
            Id = id;
        }
    }
}
