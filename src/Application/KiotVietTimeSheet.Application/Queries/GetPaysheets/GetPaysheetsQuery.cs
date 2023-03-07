using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheets
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetsQuery : QueryBase<PagingDataSource<PaysheetDto>>
    {
        public PaySheetQueryFilter PaysheetByFilter { get; set; }
        public bool IncludePaySlips { get; set; }

        public GetPaysheetsQuery(PaySheetQueryFilter paysheetByFilter, bool includePaySlips = false)
        {
            PaysheetByFilter = paysheetByFilter;
            IncludePaySlips = includePaySlips;
        }
    }
}
