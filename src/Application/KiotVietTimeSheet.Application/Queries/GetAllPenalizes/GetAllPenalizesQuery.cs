using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetAllPenalizes
{
    [RequiredPermission(TimeSheetPermission.ClockingHistory_Read)]
    public class GetAllPenalizesQuery : QueryBase<PagingDataSource<PenalizeDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetAllPenalizesQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
