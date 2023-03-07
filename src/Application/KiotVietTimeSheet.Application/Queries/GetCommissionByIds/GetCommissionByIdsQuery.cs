using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetCommissionByIds
{
    [RequiredPermission(TimeSheetPermission.Commission_Read)]
    public class GetCommissionByIdsQuery : QueryBase<List<CommissionDto>>
    {
        public List<long> Ids { get; set; }
        public GetCommissionByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
