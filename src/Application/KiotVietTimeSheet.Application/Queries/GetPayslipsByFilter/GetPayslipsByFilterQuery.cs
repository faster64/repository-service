using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipsByFilter
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetPayslipsByFilterQuery : QueryBase<PagingDataSource<PayslipDto>>
    {
        public ISqlExpression Query { get; set; }

        public GetPayslipsByFilterQuery(ISqlExpression query)
        {
            Query = query;
        }
    }
}
