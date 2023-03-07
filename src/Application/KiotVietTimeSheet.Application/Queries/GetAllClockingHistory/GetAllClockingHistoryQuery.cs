using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetAllClockingHistory
{
    [RequiredPermission(TimeSheetPermission.ClockingHistory_Read)]
    public class GetAllClockingHistoryQuery : QueryBase<PagingDataSource<ClockingHistoryDto>>
    {
        public ISqlExpression Query { get; set; }
        public GetAllClockingHistoryQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
