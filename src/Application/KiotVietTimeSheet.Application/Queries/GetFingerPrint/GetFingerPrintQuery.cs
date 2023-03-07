using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetFingerPrint
{
    public class GetFingerPrintQuery : QueryBase<PagingDataSource<FingerPrintDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetFingerPrintQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
