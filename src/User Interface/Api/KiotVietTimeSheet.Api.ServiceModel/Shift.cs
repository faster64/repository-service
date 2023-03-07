using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using ServiceStack;
using System;
using System.Collections.Generic;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/shifts",
        "GET",
        Summary = "Lấy danh sách ca làm việc",
        Notes = "")
    ]
    public class GetListShiftReq : QueryDb<Shift>, IReturn<object>
    {
        [QueryDbField(Template = "(Name LIKE {Value})", Field = "Name", ValueFormat = "%{0}%")]
        public string Keyword { get; set; }

        [QueryDbField(Template = "(BranchId = {Value})", Field = "BranchId", ValueFormat = "{0}")]
        public int? BranchId { get; set; }

        [QueryDbField(Field = "Id", Template = "{Field} IN ({Values})")]
        public List<long> IdIn { get; set; }
    }

    [Route("/shifts/{Id}",
        "GET",
        Summary = "Lấy thông tin chi tiết ca làm việc theo Id",
        Notes = "")
    ]
    public class GetShiftByIdReq : IReturn<ShiftDto>
    {
        public long Id { get; set; }
    }

    [Route("/shifts/getShiftByWorkingDayEmployee",
        "GET",
        Summary = "Lấy danh sách ca làm việc theo ngày làm việc của nhân viên",
        Notes = "")
    ]
    public class GetShiftByWorkingDayEmployeeReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
    }

    [Route("/shifts/orderby-from-to",
        "GET",
        Summary = "Lấy danh sách ca làm việc sắp xếp theo thời gian bắt đầu, thời gian kết thúc, ngày tạo mới nhất",
        Notes = "")
    ]
    public class GetShiftOrderByFromAndToReq : IReturn<List<ShiftDto>>
    {
        public List<int> BranchIds { get; set; }
        public int BranchId { get; set; }
        public List<long> ShiftIds { get; set; }
        public string Keyword { get; set; }
    }

    [Route("/shifts/multiple-branch/orderby-from-to",
        "GET",
        Summary = "Lấy danh sách ca làm việc trên nhiều chi nhánh sắp xếp theo thời gian bắt đầu, thời gian kết thúc, ngày tạo mới nhất",
        Notes = "")
    ]
    public class GetShiftMultipleBranchOrderByFromAndToReq : IReturn<List<ShiftDto>>
    {
        public List<int> BranchIds { get; set; }
        public List<long> ShiftIds { get; set; }
        public List<long> IncludeShiftIds { get; set; }
        public bool IncludeDeleted { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/shifts",
        "POST",
        Summary = "Tạo mới ca làm việc",
        Notes = "")
    ]
    public class CreateShiftReq : IReturn<object>
    {
        public bool IsGeneralSetting { get; set; }
        public ShiftDto Shift { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/shifts/{Id}",
        "PUT",
        Summary = "Cập nhật ca làm việc",
        Notes = "")
    ]
    public class UpdateShiftReq : IReturn<object>
    {
        public long Id { get; set; }
        public ShiftDto Shift { get; set; }
        public bool IsGeneralSetting { get; set; }
    }
    [Route("/shifts/change-active/{Id}", "PUT")]
    public class ChangeActiveShiftReq : IReturn<object>
    {
        public long Id { get; set; }
        public ShiftDto Shift { get; set; }
        public bool IsGeneralSetting { get; set; }
    }

    #endregion

    #region DELETE classes
    [Route("/shifts/{Id}",
        "DELETE",
        Summary = "Xóa ca làm việc",
        Notes = "")
    ]
    public class DeleteShiftReq : IReturn<object>
    {
        public long Id { get; set; }
        public bool IsGeneralSetting { get; set; }
    }
    #endregion
}
