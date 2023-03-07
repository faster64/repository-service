using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.Repositories.WriteRepositories
{
    public interface ISettingsWriteOnlyRepository : IBaseWriteOnlyRepository<Settings>
    {
        Task<bool> UpdateAutoKeepingCronScheduleAsync(Settings setting);
        Task<bool> FindAndInsertAutoKeepingCronScheduleAsync(Settings setting);
    }
}
