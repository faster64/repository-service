using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetsOldVersionByIds
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetsOldVersionByIdsQuery : QueryBase<List<PaysheetDto>>
    {
        public List<long> Ids { get; set; }

        public GetPaysheetsOldVersionByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
