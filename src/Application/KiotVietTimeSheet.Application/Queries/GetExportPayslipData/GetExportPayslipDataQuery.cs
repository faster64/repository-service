using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetExportPayslipData
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetExportPayslipDataQuery : QueryBase<List<PayslipDto>>
    {
        public PayslipByPaysheetIdFilter Filter { get; set; }

        public GetExportPayslipDataQuery(PayslipByPaysheetIdFilter fitler)
        {
            Filter = fitler;
        }
    }
}
