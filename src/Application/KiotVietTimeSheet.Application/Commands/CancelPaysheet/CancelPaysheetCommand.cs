using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CancelPaysheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Delete)]
    public class CancelPaysheetCommand : BaseCommand<PaysheetDto>
    {
        public long Id { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }

        public CancelPaysheetCommand(long id, bool isCheckPayslipPayment, bool isCancelPayment)
        {
            Id = id;
            IsCheckPayslipPayment = isCheckPayslipPayment;
            IsCancelPayment = isCancelPayment;
        }
    }
}
