using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetListConfirmClockingHistory
{
    public class GetListConfirmClockingHistoryQuery : QueryBase<PagingDataSource<ConfirmClockingHistoryDto>>
    {
        public ISqlExpression Query { get; set; }
        public bool IncludeSoftDelete { get; set; }
        public GetListConfirmClockingHistoryQuery(ISqlExpression query, bool includeSoftDelete)
        {
            Query = query;
            IncludeSoftDelete = includeSoftDelete;
        }
    }
}
