using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using ServiceStack;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Api.ServiceModel.Types;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    #region GET classes
    [Route("/paysheets",
        "GET",
        Summary = "Lấy danh sách bảng lương",
        Notes = "")
    ]
    public class GetPaysheets : PagingBaseReq, IReturn<object>
    {
        public List<long> PaySheetIds { get; set; }
        public string PaysheetKeyword { get; set; }
        public List<int?> BranchIds { get; set; }
        public List<byte> PaysheetStatuses { get; set; }
        public byte? SalaryPeriod { get; set; }
        public string EmployeeKeyword { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    [Route("/paysheets/check-exists-payslips",
        "GET",
        Summary = "Kiểm tra xem bảng lương có chứa phiếu lương nào hay không",
        Notes = "")
    ]
    public class GetCheckExistsPayslip : IReturn<object>
    {
        public long PaysheetId { get; set; }
    }

    [Route("/paysheets/working-days",
        "GET",
        Summary = "Lấy số ngày làm việc của chi nhánh trong một khoảng thời gian",
        Notes = "")
    ]
    public class GetWorkingDayNumber : IReturn<object>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    [Route("/paysheets/{Id}",
        "GET",
        Summary = "Lấy danh sách bảng lương theo Id",
        Notes = "")
    ]
    public class GetPaysheetById : QueryDb<Paysheet>, IReturn<object>
    {
        public long Id { get; set; }
    }

    [Route("/paysheets/get-and-check-change-by-id/{Id}",
        "GET",
        Summary = "Kiểm tra bảng lương có bị thay đổi không nếu có sẽ thông báo cho người dùng",
        Notes = "")
    ]
    public class GetAndCheckChangeById : IReturn<object>
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public int KvSessionBranchId { get; set; }
    }

    [Route("/paysheets/check-change-version/{Id}",
        "GET",
        Summary = "Kiểm tra bảng lương có bị thay đổi không nếu có sẽ thông báo cho người dùng",
        Notes = "")
    ]
    public class CheckChangeVersionPaysheetReq : IReturn<object>
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public long Version { get; set; }
    }

    public class GetPaysheetsOldVersionByIdsReq : IReturn<object>
    {
        public List<long> Ids { get; set; }
    }

    [Route("/paysheets/get-draft-paysheet",
        "GET",
        Summary = "Lấy bảng lương nháp",
        Notes = "")
    ]
    public class GetDraftPaysheet : IReturn<object>
    {
        public byte SalaryPeriod { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<long> EmployeeIds { get; set; }
        public int BranchId { get; set; }
        public long PaysheetId { get; set; }
        public int? WorkingDayNumber { get; set; }
    }

    [Route("/paysheets/export/data",
        "GET",
        Summary = "Lấy dữ liệu export bảng lương",
        Notes = "")
    ]
    public class GetExportPaySheetDataReq : IReturn<object>
    {
        public GetPaysheets Filters { get; set; }
    }
    #endregion

    #region POST classes
    [Route("/paysheets",
       "POST",
       Summary = "Tạo mới bảng lương",
       Notes = "")
   ]
    public class CreatePaysheet : IReturn<object>
    {
        public byte SalaryPeriod { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<BranchDto> Branches { get; set; }
    }

    [Route("/paysheets/complete",
       "POST",
       Summary = "Chốt bảng lương tạm",
       Notes = "")
    ]
    public class CompletePaysheet : IReturn<object>
    {
        public PaysheetDto Paysheet { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }
    }

    [Route("/paysheets/change-version",
        "POST",
        Summary = "thay đổi version paysheet",
        Notes = "")
    ]
    public class ChangeVersionPaysheetReq : IReturn<object>
    {
        public List<long> Ids { get; set; }
    }

    [Route("/paysheets/generate-working-period",
        "GET",
        Summary = "Lấy danh sách kì làm việc",
        Notes = "")
    ]
    public class GenerateWorkingPeriod : IReturn<object>
    {
        public int SalaryPeriodType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsUpdatePaysheet { get; set; }
    }

    [Route("/paysheets/get-paysheet-when-change-working-period",
        "POST",
        Summary = "Lấy và cập nhật lại bảng lương khi thay đổi kỳ làm việc",
        Notes = "")
    ]
    public class GetPaysheetWhenChangeWorkingPeriod : IReturn<object>
    {
        public PaysheetDto PaysheetDto { get; set; }
        public List<BranchDto> Branches { get; set; }
    }

    [Route("/paysheets/export/data",
        "POST",
        Summary = "Lấy dữ liệu export bảng lương",
        Notes = "")
    ]
    public class PostExportPaySheetDataReq : IReturn<object>
    {
        public GetPaysheets Filters { get; set; }
    }

    [Route("/paysheets/export-commission-detail",
        "POST",
        Summary = "Lấy dữ liệu export chi tiết sản phẩm theo nhân viên",
        Notes = "")
    ]
    public class PostExportPaySheetCommissionDetailDataReq : IReturn<object>
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }
        public long PayslipId { get; set; }
    }
    #endregion

    #region PUT classes
    [Route("/paysheets/{Id}",
       "PUT",
       Summary = "Cập nhật bảng lương",
       Notes = "")
    ]
    public class UpdatePaysheet : IReturn<object>
    {
        public long Id { get; set; }
        public PaysheetDto Paysheet { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }
        public bool IsGetLastestPaysheetData { get; set; }
    }

    [Route("/paysheets/cancel-paysheet",
        "PUT",
        Summary = "Hủy bỏ bảng lương",
        Notes = "")
    ]
    public class CancelPaysheetReq : IReturn<object>
    {
        public long Id { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }
    }

    [Route("/paysheets/auto-loading-and-update-data-source/{Id}",
        "PUT",
        Summary = "Cập nhật lại dữ liệu cho bảng lương",
        Notes = "")
    ]
    public class AutoLoadingAndUpdateDataPaysheetReq : IReturn<object>
    {
        public long Id { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsAcceptLoading { get; set; }
        public List<BranchDto> Branches { get; set; }
    }
    #endregion
}
