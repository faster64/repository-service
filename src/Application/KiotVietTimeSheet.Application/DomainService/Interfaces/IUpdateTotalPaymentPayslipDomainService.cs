using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService.Interfaces
{
    public interface IUpdateTotalPaymentPayslipDomainService
    {
        Task UpdateTotalPaymentWithAmoutForPayslipsAsync(Dictionary<long, decimal> dicUpdate);

        Task CancelPayslipsAndResetTotalPayment(List<Payslip> payslips);
    }
}
