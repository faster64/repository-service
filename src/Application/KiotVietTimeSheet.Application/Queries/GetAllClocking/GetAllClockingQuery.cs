using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetAllClocking
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetAllClockingQuery : QueryBase<PagingDataSource<ClockingDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetAllClockingQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
