using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetAllPenalizeByTenantId
{
    [RequiredPermission(TimeSheetPermission.Clocking_Read)]
    public class GetAllPenalizeByTenantIdQuery : QueryBase<List<PenalizeDto>>
    {
        public GetAllPenalizeByTenantIdQuery()
        {
        }
    }
}
