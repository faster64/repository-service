using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetListConfirmClocking
{
    public class GetListConfirmClockingQuery : QueryBase<PagingDataSource<ConfirmClockingDto>>
    {
        public ISqlExpression Query { get; set; }
        public bool IncludeSoftDelete { get; set; }
        public GetListConfirmClockingQuery(ISqlExpression query, bool includeSoftDelete)
        {
            Query = query;
            IncludeSoftDelete = includeSoftDelete;
        }
    }
}
