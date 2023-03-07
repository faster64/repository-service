using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Commands.ExportPaySheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Export)]
    public class ExportPaySheetCommand : QueryBase<PagingDataSource<PaysheetDto>>
    {
        public PaySheetQueryFilter PaysheetByFilter { get; set; }

        public ExportPaySheetCommand(PaySheetQueryFilter filters)
        {
            PaysheetByFilter = filters;
        }
    }
}
