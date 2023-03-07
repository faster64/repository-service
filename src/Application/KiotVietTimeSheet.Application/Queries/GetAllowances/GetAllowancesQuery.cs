using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetAllowances
{
    [RequiredPermission(TimeSheetPermission.Allowance_Read)]
    public class GetAllowancesQuery : QueryBase<PagingDataSource<AllowanceDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetAllowancesQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
