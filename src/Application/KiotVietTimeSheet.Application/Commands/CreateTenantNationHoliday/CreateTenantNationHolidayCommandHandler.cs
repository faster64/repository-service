using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Validators;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Commands.CreateTenantNationHoliday
{
    public class CreateTenantNationHolidayCommandHandler : BaseCommandHandler, IRequestHandler<CreateTenantNationHolidayCommand, List<Holiday>>
    {
        private readonly ILogger<CreateTenantNationHolidayCommandHandler> _logger;
        private readonly ITenantNationalHolidayWriteOnlyRepository _tenantNationalHolidayWriteOnlyRepository;
        private readonly ITenantNationalHolidayReadOnlyRepository _tenantNationalHolidayReadOnlyRepository;
        private readonly INationalHolidayReadOnlyRepository _nationalHolidayReadOnlyRepository;
        private readonly IHolidayReadOnlyRepository _holidayReadOnlyRepository;
        private readonly IHolidayWriteOnlyRepository _holidayWriteOnlyRepository;

        public CreateTenantNationHolidayCommandHandler(
            IEventDispatcher eventDispatcher,
            ITenantNationalHolidayWriteOnlyRepository tenantNationalHolidayWriteOnlyRepository,
            ITenantNationalHolidayReadOnlyRepository tenantNationalHolidayReadOnlyRepository,
            INationalHolidayReadOnlyRepository nationalHolidayReadOnlyRepository,
            IHolidayReadOnlyRepository holidayReadOnlyRepository,
            IHolidayWriteOnlyRepository holidayWriteOnlyRepository,
            ILogger<CreateTenantNationHolidayCommandHandler> logger
        )
            : base(eventDispatcher)
        {
            _tenantNationalHolidayWriteOnlyRepository = tenantNationalHolidayWriteOnlyRepository;
            _tenantNationalHolidayReadOnlyRepository = tenantNationalHolidayReadOnlyRepository;
            _nationalHolidayReadOnlyRepository = nationalHolidayReadOnlyRepository;
            _holidayReadOnlyRepository = holidayReadOnlyRepository;
            _holidayWriteOnlyRepository = holidayWriteOnlyRepository;
            _logger = logger;
        }

        public async Task<List<Holiday>> Handle(CreateTenantNationHolidayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var holidays = new List<Holiday>();
                var now = DateTime.Now;

                var yearIsCreated = now.Year;

                if (now.Month > request.NationalHolidayCreationMonth)
                {
                    yearIsCreated = now.AddYears(1).Year;
                }
                else if (now.Month == request.NationalHolidayCreationMonth && now.Day >= request.NationalHolidayCreationDay)
                {
                    yearIsCreated = now.AddYears(1).Year;
                }

                var existNationalHoliday = await _tenantNationalHolidayReadOnlyRepository.TenantNationalHolidayCheckAnyAsync(yearIsCreated, request.TenantId);

                if (existNationalHoliday) return holidays;

                var nationalHolidays = await _nationalHolidayReadOnlyRepository.NationHolidayGetAllAsync();

                var tenantNationalHoliday =
                    await _tenantNationalHolidayReadOnlyRepository.GetByTenantIdAsync(request.TenantId);

                holidays = GenerateHolidays(request.TenantId, request.UserId, nationalHolidays, yearIsCreated);

                await RemoveHoliday(request.TenantId, yearIsCreated, holidays);

                if (!holidays.Any()) return holidays;

                _holidayWriteOnlyRepository.BatchAddNotGuard(holidays);

                if (tenantNationalHoliday == null)
                {
                    var tenantNationHoliday = new TenantNationalHoliday(holidays.First().From.Year)
                    { TenantId = request.TenantId };
                    _tenantNationalHolidayWriteOnlyRepository.AddEntityNotGuard(tenantNationHoliday);
                }
                else
                {
                    tenantNationalHoliday.UpdateLastCreatedYear(holidays.First().From.Year);
                    _tenantNationalHolidayWriteOnlyRepository.UpdateEntityNotGuard(tenantNationalHoliday);
                }

                await _tenantNationalHolidayWriteOnlyRepository.UnitOfWork.CommitAsync();

                return holidays;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private List<Holiday> GenerateHolidays(int tenantId, long userId, List<NationalHoliday> nationalHolidays, int yearIsCreated)
        {
            var lunarCalendar = new ChineseLunisolarCalendar();

            var holidays = new List<Holiday>();
            nationalHolidays.ForEach(nationalHoliday =>
            {
                Holiday holiday;
                if (nationalHoliday.IsLunarCalendar)
                {
                    var from = lunarCalendar.ToDateTime(yearIsCreated, nationalHoliday.StartMonth,
                        nationalHoliday.StartDay, 0, 0, 0, 0);
                    var to = lunarCalendar.ToDateTime(yearIsCreated, nationalHoliday.EndMonth,
                        nationalHoliday.EndDay, 0, 0, 0, 0);
                    holiday = new Holiday(nationalHoliday.Name, @from, to);
                }
                else
                {
                    holiday = new Holiday(nationalHoliday.Name,
                        new DateTime(yearIsCreated, nationalHoliday.StartMonth, nationalHoliday.StartDay),
                        new DateTime(yearIsCreated, nationalHoliday.EndMonth, nationalHoliday.EndDay));
                }
                holiday.CreatedDate = DateTime.Now;
                holiday.DeletedBy = userId;
                holiday.TenantId = tenantId;
                // không tạo lịch sử thao tác khi tự động tạo nghỉ lễ
                holiday.ClearDomainEvents();
                holidays.Add(holiday);
            });
            return holidays;
        }

        private async Task RemoveHoliday(int tenantId, int yearIsCreated, List<Holiday> holidays)
        {
            var holidayLength = holidays.Count - 1;

            var startDayYear = Convert.ToDateTime(yearIsCreated + "-01-01");
            var yearIsCreatedPlusOne = yearIsCreated + 1;
            var startEndYear = Convert.ToDateTime(yearIsCreatedPlusOne + "-01-01");

            var existedHolidays = await _holidayReadOnlyRepository.GetByTimeAndTenantIdAsync(startDayYear, startEndYear, tenantId);

            while (holidayLength >= 0)
            {
                var holiday = holidays[holidayLength];
                holidayLength--;

                var validationResult = await new CreateOrUpdateHolidayValidator().ValidateAsync(holiday);

                if (!validationResult.IsValid)
                {
                    holidays.Remove(holiday);
                    continue;
                }

                var isOverlapHoliday = existedHolidays.Any(x => x.To >= holiday.From.Date && x.From <= holiday.To.Date);
                if (isOverlapHoliday) holidays.Remove(holiday);
            }
        }
    }
}
