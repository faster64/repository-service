using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class PayslipPaymentCreateReq
    {
        public int RetailerId { get; set; }
        public List<PayslipDto> Payslips { get; set; }
        public long UserId { get; set; }
        public int BranchId { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}
