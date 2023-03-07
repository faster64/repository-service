using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface IPayslipDetailReadOnlyRepository : IBaseReadOnlyRepository<PayslipDetail, long>
    {
    }
}
