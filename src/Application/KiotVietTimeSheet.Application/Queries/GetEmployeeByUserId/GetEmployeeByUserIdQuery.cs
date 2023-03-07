using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetEmployeeByUserId
{
    [RequiredPermission(TimeSheetPermission.Employee_Read)]
    public class GetEmployeeByUserIdQuery : QueryBase<EmployeeDto>
    {
        public ISqlExpression Query { get; set; }
        public GetEmployeeByUserIdQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
