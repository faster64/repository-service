using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.ServiceClients.RequestModels
{
    public class MakePayslipPaymentsForPaysheetReq
    {
        public PaysheetDto Paysheet { get; set; }
        public bool IsComplete { get; set; }
    }
}
