using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using ServiceStack;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/confirmclocking",
        "GET",
        Summary = "Lấy danh sách ConfirmClocking",
        Notes = "")
    ]
    public class GetListConfirmClockingReq : QueryDb<ConfirmClocking>, IReturn<object>
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

    [Route("/confirmclocking/get-confirm-clockings-by-branch-id",
       "GET",
       Summary = "Lấy danh sách ConfirmClocking for noti",
       Notes = "")
    ]
    public class GetConfirmClockingsByBranchIdReq : IReturn<object>
    {
        public int BranchId { get; set; }
    }

    [Route("/confirmclocking",
      "PUT",
      Summary = "Cập nhật GPS",
      Notes = "")
  ]
    public class UpdateConfirmClockingReq : IReturn<object>
    {
        public List<ConfirmClockingDto> lsConfirmClocking { get; set; }

    }
}
