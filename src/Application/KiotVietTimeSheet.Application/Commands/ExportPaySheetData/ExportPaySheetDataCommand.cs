using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Commands.ExportPaySheetData
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Export)]
    public class ExportPaySheetDataCommand : QueryBase<PagingDataSource<PaysheetDto>>
    {
        public PaySheetQueryFilter PaysheetByFilter { get; set; }

        public ExportPaySheetDataCommand(PaySheetQueryFilter filters)
        {
            PaysheetByFilter = filters;
        }
    }
}
