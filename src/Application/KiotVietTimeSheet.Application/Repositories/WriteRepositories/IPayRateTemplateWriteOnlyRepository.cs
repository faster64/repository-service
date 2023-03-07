using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface IPayRateTemplateWriteOnlyRepository : IBaseWriteOnlyRepository<PayRateTemplate>
    {
        Task<PayRateTemplate> UpdatePayRateTemplateAsync(PayRateTemplate template);
    }
}
