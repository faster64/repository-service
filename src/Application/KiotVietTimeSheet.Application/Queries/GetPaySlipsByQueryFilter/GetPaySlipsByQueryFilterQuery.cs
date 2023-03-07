using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetPaySlipsByQueryFilter
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetPaySlipsByQueryFilterQuery : QueryBase<PagingDataSource<PayslipDto>>
    {
        public PayslipByPaysheetIdFilter Filter { get; set; }

        public GetPaySlipsByQueryFilterQuery(PayslipByPaysheetIdFilter fitler)
        {
            Filter = fitler;
        }
    }
}
