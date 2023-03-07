using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.AppQueries.QueryFilters;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetsByFilterAsync
{
    public sealed class GetPaysheetsByFilterAsyncQuery : QueryBase<PaysheetPagingDataSource>
    {
        public PaySheetQueryFilter PaysheetByFilter { get; set; }
        public bool IncludePaySlips { get; set; }

        public GetPaysheetsByFilterAsyncQuery(PaySheetQueryFilter paysheetByFilter, bool includePaySlips = false)
        {
            PaysheetByFilter = paysheetByFilter;
            IncludePaySlips = includePaySlips;
        }
    }
}
