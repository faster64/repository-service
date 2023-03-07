using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.Domain.Common;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class UpdateTenantNationalHolidayDomainService : IUpdateTenantNationalHolidayDomainService
    {
        private readonly ITenantNationalHolidayWriteOnlyRepository _tenantNationalHolidayWriteOnlyRepository;

        public UpdateTenantNationalHolidayDomainService(ITenantNationalHolidayWriteOnlyRepository tenantNationalHolidayWriteOnlyRepository)
        {
            _tenantNationalHolidayWriteOnlyRepository = tenantNationalHolidayWriteOnlyRepository;
        }

        public async Task UpdateTenantNationalHolidayAsync(int year)
        {
            var tenantNationalHoliday =
                await _tenantNationalHolidayWriteOnlyRepository.FindBySpecificationAsync(
                    new DefaultTrueSpec<TenantNationalHoliday>());

            if (tenantNationalHoliday == null)
            {
                _tenantNationalHolidayWriteOnlyRepository.Add(new TenantNationalHoliday(year));
            }
            else
            {
                tenantNationalHoliday.UpdateLastCreatedYear(year);
                _tenantNationalHolidayWriteOnlyRepository.Update(tenantNationalHoliday);
            }
        }
    }
}
