using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;

namespace KiotVietTimeSheet.Application.Queries.GetPaysheetByIds
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Read)]
    public sealed class GetPaysheetByIdsQuery : QueryBase<List<Paysheet>>
    {
        public List<long> Ids { get; set; }

        public GetPaysheetByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
