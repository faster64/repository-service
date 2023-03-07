using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetAllowancesByIds
{
    [RequiredPermission(TimeSheetPermission.Allowance_Read)]
    public class GetAllowancesByIdsQuery : QueryBase<PagingDataSource<AllowanceDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetAllowancesByIdsQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
