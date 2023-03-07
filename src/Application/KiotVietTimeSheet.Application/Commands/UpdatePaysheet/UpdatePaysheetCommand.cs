using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdatePaysheet
{
    [RequiredPermission(TimeSheetPermission.Paysheet_Update)]
    public class UpdatePaysheetCommand : BaseCommand<PaysheetDto>
    {
        public long Id { get; set; }
        public PaysheetDto Paysheet { get; set; }
        public bool IsCheckPayslipPayment { get; set; }
        public bool IsCancelPayment { get; set; }

        public UpdatePaysheetCommand(long id, PaysheetDto paysheet, bool isCheckPayslipPayment, bool isCancelPayment)
        {
            Id = id;
            Paysheet = paysheet;
            IsCheckPayslipPayment = isCheckPayslipPayment;
            IsCancelPayment = isCancelPayment;
        }
    }
}
