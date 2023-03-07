using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Application.Repositories.ReadRepositories
{
    public interface INationalHolidayReadOnlyRepository : IBaseReadOnlyRepository<NationalHoliday, int>
    {
        Task<List<NationalHoliday>> NationHolidayGetAllAsync();
    }
}
