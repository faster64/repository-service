using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionInActiveByIds
{
    [RequiredPermission(TimeSheetPermission.Commission_Read)]
    public class GetCommissionInActiveByIdsQuery : QueryBase<List<CommissionDto>>
    {
        public List<long> Ids { get; set; }
        public GetCommissionInActiveByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
