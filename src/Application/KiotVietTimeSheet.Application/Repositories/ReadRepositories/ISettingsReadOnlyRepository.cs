using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ISettingsReadOnlyRepository : IBaseReadOnlyRepository<Settings, long>
    {
        Task<bool> IsUseClockingGps(int tenantId);
    }
}
