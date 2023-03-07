using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Application.Queries.GetAllDeductionByTenantD
{
    [RequiredPermission(TimeSheetPermission.Deduction_Read)]
    public class GetAllDeductionByTenantDQuery : QueryBase<List<DeductionDto>>
    {
        public int TenantId { get; set; }

        public GetAllDeductionByTenantDQuery(int tenantId)
        {
            TenantId = tenantId;
        }
    }
}