using System.Collections.Generic;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;

namespace KiotVietTimeSheet.Application.Queries.GetPayslipById
{
    [RequiredPermission(TimeSheetPermission.Payslip_Read)]
    public sealed class GetPayslipByIdsQuery : QueryBase<List<long>>
    {
        public List<long> Ids { get; set; }

        public GetPayslipByIdsQuery(List<long> ids)
        {
            Ids = ids;
        }
    }
}
