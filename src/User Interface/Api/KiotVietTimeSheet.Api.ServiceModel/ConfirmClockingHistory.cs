using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/confirmclockinghistory",
        "GET",
        Summary = "Lấy danh sách ConfirmClocking",
        Notes = "")
    ]
    public class GetListConfirmClockingHistoryReq : QueryDb<ConfirmClockingHistory>, IReturn<object>
    {
        public bool WithDeleted { get; set; }
        [QueryDbField(Field = "TenantId", Template = "{Field} IN ({Values})")]
        public List<long> TenantIds { get; set; }
        public string[] EmployeeNames { get; set; }
        public int?[] BranchIds { get; set; }
        public string TimeRange { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
