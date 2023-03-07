using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CancelPayslip
{
    [RequiredPermission(TimeSheetPermission.Payslip_Delete)]
    public class CancelPayslipCommand : BaseCommand<PayslipDto>
    {
        public long Id { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }

        public CancelPayslipCommand(long id, bool isCheckPayslipPayment, bool isCancelPayment)
        {
            Id = id;
            IsCheckPayslipPayment = isCheckPayslipPayment;
            IsCancelPayment = isCancelPayment;
        }
    }
}
