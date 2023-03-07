using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateNationalHoliday
{
    public class CreateNationalHolidayCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateNationalHolidayCommand, Unit>
    {
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly ITenantNationalHolidayReadOnlyRepository _tenantNationalHolidayReadOnlyRepository;
        private readonly INationalHolidayReadOnlyRepository _nationalHolidayReadOnlyRepository;
        private readonly IBatchCreateHolidayDomainService _batchCreateHolidayDomainService;

        public CreateNationalHolidayCommandHandler(
            IEventDispatcher eventDispatcher,
            IApplicationConfiguration applicationConfiguration,
            ITenantNationalHolidayReadOnlyRepository tenantNationalHolidayReadOnlyRepository,
            INationalHolidayReadOnlyRepository nationalHolidayReadOnlyRepository,
            IBatchCreateHolidayDomainService batchCreateHolidayDomainService
        )
            : base(eventDispatcher)
        {
            _applicationConfiguration = applicationConfiguration;
            _tenantNationalHolidayReadOnlyRepository = tenantNationalHolidayReadOnlyRepository;
            _nationalHolidayReadOnlyRepository = nationalHolidayReadOnlyRepository;
            _batchCreateHolidayDomainService = batchCreateHolidayDomainService;
        }

        public int GetCreatedYear()
        {
            var now = DateTime.Now;
            if (now.Month > _applicationConfiguration.NationalHolidayCreationMonth)
            {
                return now.AddYears(1).Year;
            }

            if (now.Month == _applicationConfiguration.NationalHolidayCreationMonth &&
                now.Day >= _applicationConfiguration.NationalHolidayCreationDay)
                return now.AddYears(1).Year;
            return now.Year;
        }

        public async Task<Unit> Handle(CreateNationalHolidayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                var yearIsCreated = GetCreatedYear();

                var isExistNationalHoliday = await _tenantNationalHolidayReadOnlyRepository.AnyBySpecificationAsync(
                    new FindTenantNationalHolidayByLastCreatedYearSpec(yearIsCreated));

                if (!isExistNationalHoliday)
                {
                    var lunarCalendar = new ChineseLunisolarCalendar();
                    var listNationalHoliday = await _nationalHolidayReadOnlyRepository.GetAllAsync();
                    var listHoliday = new List<Holiday>();

                    listNationalHoliday.ForEach(nationalHoliday =>
                    {
                        if (nationalHoliday.IsLunarCalendar)
                        {
                            var from = lunarCalendar.ToDateTime(yearIsCreated, nationalHoliday.StartMonth,
                                nationalHoliday.StartDay, 0, 0, 0, 0);
                            var to = lunarCalendar.ToDateTime(yearIsCreated, nationalHoliday.EndMonth,
                                nationalHoliday.EndDay, 0, 0, 0, 0);
                            var holiday = new Holiday(nationalHoliday.Name, from, to);
                            // không tạo lịch sử thao tác khi tự động tạo nghỉ lễ
                            holiday.ClearDomainEvents();
                            listHoliday.Add(holiday);
                        }
                        else
                        {
                            var holiday = new Holiday(nationalHoliday.Name,
                                new DateTime(yearIsCreated, nationalHoliday.StartMonth, nationalHoliday.StartDay),
                                new DateTime(yearIsCreated, nationalHoliday.EndMonth, nationalHoliday.EndDay));
                            // không tạo lịch sử thao tác khi tự động tạo nghỉ lễ
                            holiday.ClearDomainEvents();
                            listHoliday.Add(holiday);
                        }
                    });

                    await _batchCreateHolidayDomainService.BatchCreateAutomaticAsync(listHoliday);
                }
            }
            catch
            {
                // ignored
            }
            return Unit.Value;
        }
    }
}
