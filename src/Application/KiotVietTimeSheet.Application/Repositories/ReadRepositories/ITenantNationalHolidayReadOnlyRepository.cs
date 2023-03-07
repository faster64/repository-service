using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface ITenantNationalHolidayReadOnlyRepository : IBaseReadOnlyRepository<TenantNationalHoliday, long>
    {
        Task<TenantNationalHoliday> GetByTenantIdAsync(int tenantId);

        Task<bool> TenantNationalHolidayCheckAnyAsync(int yearIsCreated, int tenantId);
    }
}
