using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IPayslipWriteOnlyRepository : IBaseWriteOnlyRepository<Payslip>
    {
        void RemovePayslipClockings(List<PayslipClocking> payslipClockings);
        Task UpdateTotalPaymentAsync(Payslip payslip);
    }
}
