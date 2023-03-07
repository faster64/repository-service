using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CompletePaysheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Complete)]
    public class CompletePaysheetCommand : BaseCommand<PaysheetDto>
    {
        public PaysheetDto Paysheet { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }

        public CompletePaysheetCommand(PaysheetDto paysheet, bool isCheckPayslipPayment, bool isCancelPayment)
        {
            Paysheet = paysheet;
            IsCheckPayslipPayment = isCheckPayslipPayment;
            IsCancelPayment = isCancelPayment;
        }
    }
}
