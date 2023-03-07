using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/clocking-histories",
        "GET",
        Summary = "Lấy danh sách lịch sử chi tiết lịch làm việc",
        Notes = "")
    ]
    public class GetListClockingHistoryReq : QueryDb<ClockingHistory>, IReturn<object>
    {
        public int ClockingId { get; set; }
    }

    [Route("/clocking-histories/{Id}",
        "PUT",
        Summary = "Cập nhật một lịch sử chi tiết lịch làm việc",
        Notes = "")
    ]
    public class UpdateClockingHistoryReq : IReturn<object>
    {
        public long Id { get; set; }
        public ClockingHistoryDto ClockingHistory { get; set; }
        public DateTime ClockingStartTime { get; set; }
        public DateTime ClockingEndTime { get; set; }
    }

    [Route("/clocking-histories/batchUpdate",
        "POST",
        Summary = "Cập nhật danh sách lịch sử chi tiết lịch làm việc",
        Notes = "")
    ]
    public class UpdateClockingHistoriesReq : IReturn<object>
    {
        public List<ClockingHistoryDto> ClockingHistories { get; set; }
        public DateTime ClockingStartTime { get; set; }
        public DateTime ClockingEndTime { get; set; }
    }



}
