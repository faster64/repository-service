using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IPayRateWriteOnlyRepository : IBaseWriteOnlyRepository<PayRate>
    {
        Task<PayRate> UpdatePayRateAsync(PayRate payRate);
        Task<List<PayRate>> BatchUpdatePayRateAsync(List<PayRate> payRates);
    }
}
