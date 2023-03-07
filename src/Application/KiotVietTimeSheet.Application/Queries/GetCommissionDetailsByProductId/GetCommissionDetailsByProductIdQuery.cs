using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionDetailsByProductId
{
    public class GetCommissionDetailsByProductIdQuery : QueryBase<PagingDataSource<CommissionDetailDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetCommissionDetailsByProductIdQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
