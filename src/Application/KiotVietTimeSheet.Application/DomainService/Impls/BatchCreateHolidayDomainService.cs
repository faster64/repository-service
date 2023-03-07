using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Validators;

namespace KiotVietTimeSheet.Application.DomainService.Impls
{
    public class BatchCreateHolidayDomainService : IBatchCreateHolidayDomainService
    {
        #region Properties
        private readonly IHolidayWriteOnlyRepository _holidayWriteOnlyRepository;
        private readonly IUpdateTenantNationalHolidayDomainService _updateTenantNationalHolidayDomainService;
        #endregion

        public BatchCreateHolidayDomainService(
            IHolidayWriteOnlyRepository holidayWriteOnlyRepository,
            IUpdateTenantNationalHolidayDomainService updateTenantNationalHolidayDomainService)
        {
            _holidayWriteOnlyRepository = holidayWriteOnlyRepository;
            _updateTenantNationalHolidayDomainService = updateTenantNationalHolidayDomainService;
        }

        public async Task BatchCreateAutomaticAsync(List<Holiday> holidays)
        {
            var holidayLength = holidays.Count - 1;

            while (holidayLength >= 0)
            {
                var holiday = holidays[holidayLength];
                holidayLength--;

                var validationResult = await new CreateOrUpdateHolidayValidator().ValidateAsync(holiday);

                if (!validationResult.IsValid)
                {
                    holidays.Remove(holiday);
                }
                else
                {
                    var isOverlapHoliday = await _holidayWriteOnlyRepository.AnyBySpecificationAsync(
                        new FindHolidayByToGreaterThanOrEqualSpec(holiday.From)
                            .And(new FindHolidayByFromLessThanOrEqualSpec(holiday.To)));
                    if (isOverlapHoliday) holidays.Remove(holiday);
                }
            }

            if (holidays.Any())
            {
                _holidayWriteOnlyRepository.BatchAdd(holidays);
                await _updateTenantNationalHolidayDomainService.UpdateTenantNationalHolidayAsync(holidays.First().From.Year);
                await _holidayWriteOnlyRepository.UnitOfWork.CommitAsync();
            }
        }
    }
}
