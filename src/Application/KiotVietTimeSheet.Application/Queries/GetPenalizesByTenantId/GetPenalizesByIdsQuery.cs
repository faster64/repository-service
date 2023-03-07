using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPenalizesByTenantId
{
    [RequiredPermission(TimeSheetPermission.ClockingHistory_Read)]
    public class GetPenalizesByIdsQuery : QueryBase<List<PenalizeDto>>
    {
        public List<long> PenalizeId { get; set; }

        public GetPenalizesByIdsQuery(List<long> penalizeId)
        {
            PenalizeId = penalizeId;
        }
    }
}
