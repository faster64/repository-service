using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetDepartment
{
    [RequiredPermission(TimeSheetPermission.Department_Read)]
    public class GetDepartmentQuery : QueryBase<PagingDataSource<DepartmentDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetDepartmentQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
