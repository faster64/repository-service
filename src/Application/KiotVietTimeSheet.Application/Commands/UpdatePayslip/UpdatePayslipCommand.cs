using KiotVietTimeSheet.Application.Abstractions;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.UpdatePayslip
{
    public class UpdatePayslipCommand : BaseCommand<Unit>
    {
        public long PayslipId { get; set; }
        public decimal? TotalPayment { get; set; }
        public UpdatePayslipCommand(long payslipId, decimal? totalPayment)
        {
            PayslipId = payslipId;
            TotalPayment = totalPayment;
        }
    }
}
