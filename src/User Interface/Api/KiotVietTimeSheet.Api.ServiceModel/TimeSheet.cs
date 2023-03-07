using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using ServiceStack;
using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/timesheets",
        "GET",
        Summary = "Lấy danh sách lịch làm việc",
        Notes = "")
    ]
    public class GetListTimeSheetReq : QueryDb<TimeSheet>, IReturn<object>
    {
        public string Keyword { get; set; }

        public long EmployeeId { get; set; }

        public byte TimeSheetStatus { get; set; }

        public bool? IsDeleted { get; set; }
    }

    [Route("/timesheets/{Id}",
        "GET",
        Summary = "Lấy thông tin lịch làm việc theo id truyền vào",
        Notes = "")
    ]
    public class GetTimeSheetByIdReq : IReturn<object>
    {
        public long Id { get; set; }
        public bool IncludeReferences { get; set; }
    }

    [Route("/timesheets/export/data",
        "GET",
        Summary = "Lấy dữ liệu export chấm công",
        Notes = "")
    ]
    public class GetExportTimeSheetDataReq : IReturn<object>
    {
        public GetClockingForCalendarReq Filters { get; set; }
        public List<long> ShiftForCalendarIds { get; set; }
        public List<long> EmployeeForCalendarIds { get; set; }
    }

    [Route("/timesheets/export-by-branches/data",
        "GET",
        Summary = "Lấy dữ liệu export chấm công",
        Notes = "")
    ]
    public class GetExportTimeSheetMultiBranchDataReq : IReturn<object>
    {
        public GetClockingMultipleBranchForCalendarReq Filters { get; set; }
        public List<long> ShiftForCalendarIds { get; set; }
        public List<long> EmployeeForCalendarIds { get; set; }
    }
    [Route("/timesheets/export/data",
        "POST",
        Summary = "Lấy dữ liệu export chấm công",
        Notes = "")
    ]
    public class PostExportTimeSheetDataReq : IReturn<object>
    {
        public GetClockingForCalendarReq Filters { get; set; }
        public List<long> ShiftForCalendarIds { get; set; }
        public List<long> EmployeeForCalendarIds { get; set; }
    }
    [Route("/timesheets/check-timesheet-feature", "GET",
        Summary = "Kiểm tra tính năng timesheet đang được bật hay không",
        Notes = "")]
    public class CheckActiveTimesheetReq : IReturn<object>
    {
    }
    [Route("/timesheet/get-version-app", "GET")]
    public class GetVersionTimeSheetApp : IReturn<object>
    {

    }
    #endregion

    #region POST classes
    [Route("/timesheets",
        "POST",
        Summary = "Tạo mới lịch làm việc",
        Notes = "")
    ]
    public class CreateTimeSheetReq : IReturn<object>
    {
        public TimeSheetDto TimeSheet { get; set; }
    }

    [Route("/timesheets/batchAddTimeSheetWhenCreateMultipleTimeSheet",
        "POST",
        Summary = "Tạo mới danh sách lịch làm việc",
        Notes = "")
    ]
    public class BatchAddTimeSheetWhenCreateMultipleTimeSheetReq : IReturn<object>
    {
        public TimeSheetDto TimeSheet { get; set; }
        public List<long> EmployeeIds { get; set; }
        public bool IsAuto { get; set; }
    }

    [Route("/timesheets/copy",
        "POST",
        Summary = "Sao chép lịch làm việc",
        Notes = "")
    ]
    public class CopyListTimeSheetReq : IReturn<object>
    {
        public int BranchId { get; set; }

        public DateTime CopyFrom { get; set; }

        public DateTime CopyTo { get; set; }

        public DateTime PasteFrom { get; set; }

        public DateTime PasteTo { get; set; }
    }

    [Route("/timesheets/export-by-branches/data",
        "POST",
        Summary = "Lấy dữ liệu export chấm công",
        Notes = "")
    ]
    public class PostExportTimeSheetMultiBranchDataReq : IReturn<object>
    {
        public GetClockingMultipleBranchForCalendarReq Filters { get; set; }
        public List<long> ShiftForCalendarIds { get; set; }
        public List<long> EmployeeForCalendarIds { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/timesheets/{Id}",
        "PUT",
        Summary = "Cập nhật lịch làm việc",
        Notes = "")
    ]
    public class UpdateTimeSheetReq : IReturn<object>
    {
        public long Id { get; set; }
        public TimeSheetDto TimeSheet { get; set; }
        public bool ForAllClockings { get; set; }
    }

    [Route("/timesheets/cancelTimeSheet",
        "PUT",
        Summary = "Hủy lịch làm việc",
        Notes = "")
    ]
    public class CancelTimeSheetReq : IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/timesheets/sendEmailActiveTrial",
        "POST",
        Summary = "Gửi email",
        Notes = "")
    ]
    public class SendEmailActiveTrialTimeSheetReq : IReturn<object>
    {
        public string SendTo { get; set; }
        public string BccEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    #endregion
}
