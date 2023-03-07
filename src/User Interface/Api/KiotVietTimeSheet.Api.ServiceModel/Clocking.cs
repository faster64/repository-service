using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    /// <summary>
    /// 
    /// </summary>
    [Route("/clockings/getClockingsByTimeSheetId/{TimeSheetId}",
         "GET",
         Summary = "Lấy danh sách chi tiết làm việc theo lịch làm việc",
         Notes = "")
     ]
    public class GetClockingsByTimeSheetIdReq : IReturn<object>
    {
        public long TimeSheetId { get; set; }
    }

    /// <summary>
    /// Get clocking by shift id
    /// </summary>
    [Route("/clockings/getClockingsByShiftId/{ShiftId}",
        "GET",
        Summary = "Lấy danh sách chi tiết làm việc theo ca làm việc",
        Notes = "")
    ]
    public class GetClockingsByShiftIdReq : IReturn<object>
    {
        public long ShiftId { get; set; }
    }

    [Route("/clockings/get-clocking-for-calendar",
         "GET",
         Summary = "",
         Notes = "")
    ]
    public class GetClockingForCalendarReq : QueryDb<Clocking>, IReturn<object>
    {
        [QueryDbField(Operand = ">=")]
        public DateTime StartTime { get; set; }
        [QueryDbField(Field = "StartTime", Template = "{Field} <= ({Value})")]
        public DateTime EndTime { get; set; }
        public int BranchId { get; set; }
        [QueryDbField(Field = "ClockingStatus", Template = "{Field} IN ({Values})")]
        public List<byte> ClockingStatusIn { get; set; }
        public List<byte> ClockingHistoryStates { get; set; }
        [QueryDbField(Field = "Id", Template = "{Field} IN ({Values})")]
        public List<long> IdIn { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long> EmployeeIds { get; set; }
        public List<long> DepartmentIds { get; set; }
        public List<long> BranchIds { get; set; }
    }

    [Route("/clockings/get-clocking-multiple-branch-for-calendar",
        "GET",
        Summary = "",
        Notes = "")
    ]
    public class GetClockingMultipleBranchForCalendarReq : IReturn<object>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int BranchId { get; set; }
        public List<byte> ClockingStatusExtension { get; set; }
        public List<byte> ClockingHistoryStates { get; set; }
        public List<long> IdIn { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long> EmployeeIds { get; set; }
        public List<long> DepartmentIds { get; set; }
        public List<int> BranchIds { get; set; }
        public TypeCalendarView TypeCalendar { get; set; }
    }

    [Route("/clockings/{Id}",
        "GET",
        Summary = "Lấy chi tiết làm việc",
        Notes = "")
    ]
    public class GetClockingByIdReq : QueryDb<Clocking>, IReturn<object>
    {
        public long Id { get; set; }
    }

    /// <summary>
    ///  Lấy danh sách clocking của nhân viên trong một ngày làm việc xác định
    /// </summary>
    [Route("/clockings/get-clocking-for-swap",
        "GET",
        Summary = "Lấy danh sách chi tiết làm việc của nhân viên trong một ngày làm việc",
        Notes = "")
    ]
    public class GetClockingForSwapReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public DateTime Day { get; set; }
        public long? BranchId { get; set; }
        public long ShiftId { get; set; }
    }

    [Route("/clockings/get-clocking-for-payslip-clocking-detail",
        "GET",
        Summary = "Lấy danh sách chi tiết làm việc đã chấm công theo employee id cho chi tiết chấm công của phiếu lương",
        Notes = "")
    ]
    public class GetClockingForPaySlipClockingDetailReq : QueryDb<Clocking>, IReturn<object>
    {
        [QueryDbField(Field = "StartTime", Template = "{Field} >= ({Value})")]
        public DateTime PaysheetStartTime { get; set; }

        [QueryDbField(Field = "StartTime", Template = "{Field} < ({Value})")]
        public DateTime PaysheetEndTime { get; set; }

        public long EmployeeId { get; set; }
    }

    [Route("/clockings/getClockingsByBranchId/{BranchId}",
        "GET",
        Summary = "Lấy danh sách chi tiết làm việc theo lịch làm việc",
        Notes = "")
    ]
    public class GetClockingsByBranchIdReq : IReturn<object>
    {
        public int BranchId { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/clockings/cancelClockingByIds",
        "POST",
        Summary = "Hủy chi tiết làm việc theo ids",
        Notes = "")
    ]
    public class RejectClockingByIdsReq : IReturn<object>
    {
        public List<long> Ids { get; set; }
    }

    [Route("/clockings/cancelMultipleClockings",
        "POST",
        Summary = "Hủy nhiều chi tiết làm việc",
        Notes = "")
    ]
    public class RejectClockingsByFilterReq : IReturn<object>
    {
        public long BranchId { get; set; }
        public List<long> EmployeeIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ShiftId { get; set; }
        public List<byte> StatusesExtension { get; set; }
    }

    [Route("/clockings/cancelMultipleClockingsbyMultipleBranch",
        "POST",
        Summary = "Hủy nhiều chi tiết làm việc",
        Notes = "")
    ]
    public class RejectClockingsByBranchesReq : IReturn<object>
    {
        public List<long> BranchIds { get; set; }
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte Statuses { get; set; }

        public bool IforAllClockings { get; set; }
    }


    [Route("/clockings/swapshift",
        "POST",
        Summary = "Cập nhật chi tiết làm việc",
        Notes = "")
    ]
    public class SwapShiftClockingReq
    {
        public long ClockingId1 { get; set; }
        public long ClockingId2 { get; set; }
        public long EmployeeId1 { get; set; }
        public long EmployeeId2 { get; set; }
    }

    [Route("/clockings/batchUpdateClockingStatusAndBatchAddClockingHistories",
        "POST",
        Summary = "Chấm công thủ công cho danh sách chi tiết làm việc",
        Notes = "Cập nhật trạng thái danh sách chi tiết làm việc và thêm mới danh sách lịch sử chi tiết làm việc")
    ]
    public class BatchUpdateClockingStatusAndBatchAddClockingHistoriesReq : IReturn<object>
    {
        public List<ClockingDto> Clockings { get; set; }
        public ClockingHistoryDto ClockingHistory { get; set; }
        public bool LeaveOfAbsence { get; set; }
    }

    [Route("/clockings/automatedTimeKeeping",
        "POST",
        Summary = "Chấm công tự động cho một chi tiết làm việc",
        Notes = "Cập nhật trạng thái chi tiết làm việc và thêm mới lịch sử chi tiết làm việc")
    ]
    public class AutomatedTimeKeepingReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public DateTime TimeKeepingDate { get; set; }

    }

    [Route("/clockings/automatedMultipleTimeKeeping",
        "POST",
        Summary = "Chấm công tự động cho danh sách chi tiết làm việc",
        Notes = "Cập nhật trạng thái danh sách chi tiết làm việc và thêm mới danh sách lịch sử chi tiết làm việc")
    ]
    public class AutomatedMultipleTimeKeepingReq : IReturn<object>
    {
        public List<AutomatedTimekeepingDto> AutomatedTimekeepingDtos { get; set; }
    }

    [Route("/clockings/getBranchesWithDeletePermissionHaveAnyClocking",
        "GET",
        Summary = "Lấy branches theo quyền xóa và kiểm tra có hủy chi tiết ca làm việc khi bỏ chọn chi nhánh làm việc",
        Notes = "")
    ]
    public class GetBranchesWithDeletePermissionHaveAnyClocking : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public List<int> BranchCancelIds { get; set; }
    }

    [Route("/clockings/create-penalize/{ClockingId}",
        "POST",
        Summary = "Thêm mới vi phạm",
        Notes = "")
    ]
    public class CreatePenalize : IReturn<object>
    {
        public long ClockingId { get; set; }
        public ClockingPenalizeDto ClockingPenalizeDto { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/clockings/updateClockingShiftAndDateTime/{Id}",
        "PUT",
        Summary = "Cập nhật ca và thời gian chi tiết làm việc khi kéo thả trên calendar",
        Notes = "")
    ]
    public class UpdateClockingShiftAndDateTimeReq
    {
        public long Id { get; set; }
        public long ShiftTargetId { get; set; }
        public int BranchId { get; set; }
        public DateTime WorkingDay { get; set; }
    }

    [Route("/clockings/{Id}",
       "PUT",
       Summary = "Cập nhật ca, ngày, note clocking",
       Notes = "")
    ]
    public class UpdateClockingReq
    {
        public long Id { get; set; }
        public ClockingDto Clocking { get; set; }
        public long ReplacementEmployeeId { get; set; }
        public ClockingHistoryDto ClockingHistory { get; set; }
        public bool LeaveOfAbsence { get; set; }

    }
    #endregion
}
