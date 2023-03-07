using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetListTimesheet
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Read)]
    public sealed class GetListTimesheetQuery : QueryBase<PagingDataSource<TimeSheetDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetListTimesheetQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
