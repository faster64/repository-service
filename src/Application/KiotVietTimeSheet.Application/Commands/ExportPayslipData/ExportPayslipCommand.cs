using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.ExportPayslipData
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public class ExportPayslipDataCommand : QueryBase<List<PayslipDto>>
    {
        public PayslipByPaysheetIdFilter PayslipByPaysheetIdFilter { get; set; }

        public ExportPayslipDataCommand(PayslipByPaysheetIdFilter filters)
        {
            PayslipByPaysheetIdFilter = filters;
        }
    }
}
