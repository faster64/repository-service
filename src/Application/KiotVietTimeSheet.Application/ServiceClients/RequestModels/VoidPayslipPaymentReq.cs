using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class VoidPayslipPaymentReq
    {
        public List<PayslipDto> Payslips { get; set; }
        public bool IsVoidPayslipPayment { get; set; }
        public bool IsCancelPaysheet { get; set; }
        public bool IsUpdatePaysheetTracking { get; set; }
    }
}
