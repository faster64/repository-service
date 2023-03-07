using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayRateTemplateDetailReadOnlyRepository : IBaseReadOnlyRepository<PayRateTemplateDetail, long>
    {
    }
}
