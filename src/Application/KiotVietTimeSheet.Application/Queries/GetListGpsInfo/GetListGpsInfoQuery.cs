using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.SharedKernel.Models;
using ServiceStack.OrmLite;

namespace KiotVietTimeSheet.Application.Queries.GetListGpsInfo
{
    [RequiredPermission(TimeSheetPermission.TimeSheet_Read)]
    public class GetListGpsInfoQuery : QueryBase<PagingDataSource<GpsInfoDto>>
    {
        public ISqlExpression Query { get; set; }
        public bool IncludeSoftDelete { get; set; }
        public GetListGpsInfoQuery(ISqlExpression query, bool includeSoftDelete)
        {
            Query = query;
            IncludeSoftDelete = includeSoftDelete;
        }
    }
}
