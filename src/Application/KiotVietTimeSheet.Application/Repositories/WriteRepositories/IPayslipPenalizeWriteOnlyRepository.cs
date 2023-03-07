using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IPayslipPenalizeWriteOnlyRepository : IBaseWriteOnlyRepository<PayslipPenalize>
    {
        Task DeleteAllAsync(List<long> payslipIds);
    }
}
