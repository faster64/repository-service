using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipsByPaysheetId
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetPayslipsByPaysheetIdQuery : QueryBase<PagingDataSource<PayslipDto>>
    {
        public PayslipByPaysheetIdFilter Filter { get; set; }

        public GetPayslipsByPaysheetIdQuery(PayslipByPaysheetIdFilter filter)
        {
            Filter = filter;
        }
    }
}
