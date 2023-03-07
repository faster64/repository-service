using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetDeductionByIds
{
    [RequiredPermission(TimeSheetPermission.Deduction_Read)]
    public class GetDeductionByIdsQuery : QueryBase<PagingDataSource<DeductionDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetDeductionByIdsQuery(ISqlExpression query)
        {
            Query = query;
        }

    }
}
