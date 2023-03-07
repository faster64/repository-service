using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Queries.GetAllowanceByTenantId
{
    [RequiredPermission(TimeSheetPermission.Allowance_Read)]
    public class GetAllowanceByTenantIdQuery : QueryBase<List<AllowanceDto>>
    {
        public int TenantId { get; set; }

        public GetAllowanceByTenantIdQuery(int tenantId)
        {
            TenantId = tenantId;
        }
    }
}
