using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetPayRate
{
    [RequiredPermission(TimeSheetPermission.PayRate_Read)]
    public class GetPayRateQuery : QueryBase<PagingDataSource<PayRateDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetPayRateQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
