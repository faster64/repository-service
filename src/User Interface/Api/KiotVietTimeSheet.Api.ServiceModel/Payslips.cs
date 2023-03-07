using ServiceStack;
using System.Collections.Generic;
using KiotVietTimeSheet.Api.ServiceModel.Types;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.EventBus;

namespace KiotVietTimeSheet.Api.ServiceModel
{
    [Route("/payslip/getPayslipsByFilter",
        "GET",
        Summary = "Lấy danh sách phiếu lương theo filter",
        Notes = "")
    ]
    public class GetPayslipsByFilterReq : QueryDb<Payslip>, IReturn<object>
    {
        [QueryDbField(Template = "(EmployeeId = {Value})", Field = "EmployeeId")]
        public long EmployeeId { get; set; }

        [QueryDbField(Template = "(PayslipStatus = {Value})", Field = "PayslipStatus")]
        public byte? PayslipStatus { get; set; }
    }

    [Route("/payslip/allowances",
       "GET",
       Summary = "Lấy danh sách phụ cấp của một phiếu lương",
       Notes = "")
    ]
    public class GetPayslipAllowanceReq : IReturn<object>
    {
        public long PayslipId { get; set; }
    }

    [Route("/payslip/getPayslipsByPaysheetId",
        "GET",
        Summary = "Lấy danh sách phiếu lương của bảng lương",
        Notes = "")
    ]
    public class GetPayslipsByPaysheetIdReq : PagingBaseReq, IReturn<object>
    {
        public List<byte> PayslipStatuses { get; set; }
        public long PaysheetId { get; set; }
    }

    [Route("/payslip/getPayslipsClockingByPayslipId",
        "GET",
        Summary = "Lấy danh sách clocking của phiếu lương",
        Notes = "")
    ]
    public class GetPayslipsClockingByPayslipIdReq : PagingBaseReq, IReturn<object>
    {
        public long PayslipId { get; set; }
    }

    [Route("/payslip/export/data",
        "GET",
        Summary = "Lấy dữ liệu export chấm công",
        Notes = "")
    ]
    public class GetExportPayslipDataReq : IReturn<object>
    {
        public GetPaySlipsByQueryFilter Filters { get; set; }
    }

    [Route("/payslips",
        "GET",
        Summary = "Lấy danh sách phiếu lương theo điều kiện lọc",
        Notes = "")
    ]
    public class GetPaySlipsByQueryFilter : PagingBaseReq, IReturn<object>
    {
        public byte[] PayslipStatuses { get; set; }
        public long? PaysheetId { get; set; }
        public long? EmployeeId { get; set; }
        public List<long> PayslipIds { get; set; }
    }

    [Route("/payslips/{EmployeeId}/unpaid-payslips",
        "GET",
        Summary = "Get danh sách phiếu lương chưa thánh toán theo employee id truyền vào",
        Notes = "")
    ]
    public class GetUnPaidPayslipByEmployeeIdReq : IReturn<object>
    {
        public long EmployeeId { get; set; }
    }

    [Route("/payslip/cancel-payslip",
        "PUT",
        Summary = "Hủy phiếu lương",
        Notes = "")
    ]
    public class CancelPayslipReq : IReturn<object>
    {
        public long Id { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }
    }

    [Route("/payslips/{Id}",
        "GET",
        Summary = "Lấy chi tiết phiếu lương",
        Notes = "")
    ]
    public class GetPaySlipById : IReturn<object>
    {
        public long Id { get; set; }
    }

    #region POST Classes
    [Route("/payslip/export/data",
        "POST",
        Summary = "Lấy dữ liệu export chấm công",
        Notes = "")
    ]
    public class PostExportPayslipDataReq : IReturn<object>
    {
        public GetPaySlipsByQueryFilter Filters { get; set; }
    }

    [Route("/payslip/export/employee-payslip-data",
        "POST",
        Summary = "Lấy dữ liệu export phiếu lương nhân viên",
        Notes = "")
    ]
    public class PostExportEmployeePayslipDataReq : IReturn<object>
    {
        public long PaysheetId { get; set; }
        public int BranchId { get; set; }
        public long EmployeeId { get; set; }
        public long PayslipId { get; set; }

    }
    #endregion

    #region Internal Api
    [Route("/payslip/created-payslip-event", "POST")]
    public class CreatedPayslipPaymentEventReq : IReturn<object>
    {
        public CreatedPayslipPaymentIntegrationEvent Event { get; set; }
    }

    [Route("/payslip/voided-payslip-payment", "POST")]
    public class VoidPayslipPaymentEventReq : IReturn<object>
    {
        public VoidedPayslipPaymentIntegrationEvent Event { get; set; }
    }
    #endregion
}
