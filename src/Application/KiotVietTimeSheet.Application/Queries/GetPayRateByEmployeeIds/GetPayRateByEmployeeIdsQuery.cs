using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Application.Queries.GetPayRateByEmployeeIds
{
    [RequiredPermission(TimeSheetPermission.PayRate_Read)]
    public class GetPayRateByEmployeeIdsQuery : QueryBase<List<PayRate>>
    {
        public List<long> EmployeeIds { get; set; }

        public GetPayRateByEmployeeIdsQuery(List<long> employeeIds)
        {
            EmployeeIds = employeeIds;
        }
    }
}
