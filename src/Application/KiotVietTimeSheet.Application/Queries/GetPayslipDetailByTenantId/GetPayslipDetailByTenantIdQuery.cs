using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipDetailByTenantId
{
    [RequiredPermission(TimeSheetPermission.PayslipDetail_Read)]
    public sealed class GetPayslipDetailByTenantIdQuery : QueryBase<List<PayslipDetailDto>>
    {
        public int TenantId { get; set; }
        public string RuleType { get; set; }

        public GetPayslipDetailByTenantIdQuery(int tenantId, string ruleType)
        {
            TenantId = tenantId;
            RuleType = ruleType;
        }
    }
}
