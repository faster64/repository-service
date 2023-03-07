using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetSetting;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetGenerateWorkingPeriod
{
    public class GenerateWorkingPeriodQueryHandler : BaseCommandHandler,
        IRequestHandler<GenerateWorkingPeriodQuery, List<PaySheetWorkingPeriodDto>>
    {
        private const int Period = 20;
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;

        public GenerateWorkingPeriodQueryHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMediator mediator
        )
            : base(eventDispatcher)
        {
            _mediator = mediator;
            _authService = authService;
        }

        public async Task<List<PaySheetWorkingPeriodDto>> Handle(GenerateWorkingPeriodQuery request, CancellationToken cancellationToken)
        {
            var settingObjectDto = await _mediator.Send(new GetSettingQuery(_authService.Context.TenantId), cancellationToken);
            var salaryPeriodType = request.SalaryPeriodType;
            var isUpdatePaysheet = request.IsUpdatePaysheet;
            var paySheetWorkingPeriodDtos = new List<PaySheetWorkingPeriodDto>();
            switch (salaryPeriodType)
            {
                case (int)PaySheetWorkingPeriodStatuses.EveryMonth:
                    paySheetWorkingPeriodDtos = GenerateWorkingPeriodsByEveryMonth(settingObjectDto.StartDateOfEveryMonth, !isUpdatePaysheet ? (DateTime?)null : request.StartDate);
                    break;
                case (int)PaySheetWorkingPeriodStatuses.TwiceAMonth:
                    paySheetWorkingPeriodDtos = GenerateWorkingPeriodsByTwiceAMonth(
                        settingObjectDto.FirstStartDateOfTwiceAMonth,
                        settingObjectDto.SecondStartDateOfTwiceAMonth,
                        !isUpdatePaysheet ? (DateTime?)null : request.StartDate,
                        !isUpdatePaysheet ? (DateTime?)null : request.EndDate);
                    break;
                case (int)PaySheetWorkingPeriodStatuses.EveryWeek:
                    paySheetWorkingPeriodDtos = GenerateWorkingPeriodsByEveryWeek(settingObjectDto.StartDayOfWeekEveryWeek, !isUpdatePaysheet ? (DateTime?)null : request.StartDate);
                    break;
                case (int)PaySheetWorkingPeriodStatuses.TwiceWeekly:
                    paySheetWorkingPeriodDtos = GenerateWorkingPeriodsByTwiceWeekly(settingObjectDto.StartDayOfWeekTwiceWeekly, !isUpdatePaysheet ? (DateTime?)null : request.StartDate);
                    break;
                case (int)PaySheetWorkingPeriodStatuses.Option:
                    paySheetWorkingPeriodDtos = GenerateWorkingPeriodsByOption(request.StartDate, request.EndDate);
                    break;
            }
            var result = await Task.FromResult(paySheetWorkingPeriodDtos);
            return result;
        }

        private List<PaySheetWorkingPeriodDto> GenerateWorkingPeriodsByOption(DateTime startDate, DateTime endDate)
        {
            var paySheetWorkingPeriodDtos = new List<PaySheetWorkingPeriodDto>();
            var paySheetWorkingPeriodDto = new PaySheetWorkingPeriodDto
            {
                Id = 1,
                Name = startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"),
                StartTime = startDate.Date,
                EndTime = endDate.Date.AddDays(1).AddSeconds(-1)
            };
            paySheetWorkingPeriodDtos.Add(paySheetWorkingPeriodDto);

            return paySheetWorkingPeriodDtos;
        }

        private PaySheetWorkingPeriodDto GenerateWorkingPeriodByTwiceWeekly(int startDayOfWeekTwiceWeekly,
            DateTime currentPeriodTime, int index, DayOfWeek currentDayOfWeek)
        {
            var startDate = currentPeriodTime.AddDays(startDayOfWeekTwiceWeekly - (byte)currentDayOfWeek);
            var endDate = startDate.AddDays(14).Date.AddSeconds(-1);

            return new PaySheetWorkingPeriodDto
            {
                Id = index / 2 + 1,
                Name = startDate.ToString("dd/MM/yyyy") + " - " + endDate.Date.ToString("dd/MM/yyyy"),
                StartTime = startDate,
                EndTime = endDate
            };
        }

        private List<PaySheetWorkingPeriodDto> GenerateWorkingPeriodsByEveryWeek(byte startDayOfWeekEveryWeek, DateTime? paysheetStartDate)
        {
            var now = DateTime.Now;
            var nextWeek = now.AddDays(7).Date;
            var currentDayOfWeek = now.DayOfWeek;
            var listPaySheetWorkingPeriodDtos = new List<PaySheetWorkingPeriodDto>();
            for (var i = 0; i < Period; i++)
            {
                var currentPeriodTime = nextWeek.AddDays(-i * 7).Date;
                var paySheetWorkingPeriodDto = GenerateWorkingPeriodByEveryWeek(startDayOfWeekEveryWeek,
                    currentPeriodTime, i, currentDayOfWeek);
                listPaySheetWorkingPeriodDtos.Add(paySheetWorkingPeriodDto);
            }

            listPaySheetWorkingPeriodDtos = listPaySheetWorkingPeriodDtos.OrderByDescending(p => p.StartTime).ToList();
            if (paysheetStartDate != null && startDayOfWeekEveryWeek != (byte)paysheetStartDate.Value.DayOfWeek)
            {
                listPaySheetWorkingPeriodDtos.Insert(0, GenerateWorkingPeriodByEveryWeek((byte)paysheetStartDate.Value.DayOfWeek,
                    paysheetStartDate.Value, 100, paysheetStartDate.Value.DayOfWeek));
            }
            return listPaySheetWorkingPeriodDtos;
        }

        private List<PaySheetWorkingPeriodDto> GenerateWorkingPeriodsByEveryMonth(int startDateOfEveryMonth, DateTime? paysheetStartDate)
        {
            var now = DateTime.Now;
            var nextMonth = now.AddMonths(1);
            var paySheetWorkingPeriodDtos = new List<PaySheetWorkingPeriodDto>();

            for (var i = 0; i < Period; i++)
            {
                var currentPeriodTime = nextMonth.AddMonths(-i);
                paySheetWorkingPeriodDtos.Add(GenerateWorkingPeriodByEveryMonth(startDateOfEveryMonth, currentPeriodTime, i));
            }

            paySheetWorkingPeriodDtos = paySheetWorkingPeriodDtos.OrderByDescending(p => p.StartTime).ToList();
            if (paysheetStartDate != null && startDateOfEveryMonth != paysheetStartDate.Value.Day)
            {
                paySheetWorkingPeriodDtos.Insert(0,
                    GenerateWorkingPeriodByEveryMonth(paysheetStartDate.Value.Day, paysheetStartDate.Value, 100));
            }
            return paySheetWorkingPeriodDtos;
        }

        private List<PaySheetWorkingPeriodDto> GenerateWorkingPeriodsByTwiceWeekly(int startDayOfWeekTwiceWeekly, DateTime? paysheetStartDate)
        {
            var now = DateTime.Now;
            var nextWeek = now.AddDays(7).Date;
            var currentDayOfWeek = now.DayOfWeek;
            var listPaySheetWorkingPeriodDtos = new List<PaySheetWorkingPeriodDto>();
            for (var i = 0; i < Period * 2; i += 2)
            {
                var currentPeriodTime = nextWeek.AddDays(-i * 7).Date;
                var paySheetWorkingPeriodDto = GenerateWorkingPeriodByTwiceWeekly(startDayOfWeekTwiceWeekly,
                    currentPeriodTime, i, currentDayOfWeek);
                listPaySheetWorkingPeriodDtos.Add(paySheetWorkingPeriodDto);
            }

            listPaySheetWorkingPeriodDtos = listPaySheetWorkingPeriodDtos.OrderByDescending(p => p.StartTime).ToList();
            if (paysheetStartDate != null && startDayOfWeekTwiceWeekly != (byte)paysheetStartDate.Value.DayOfWeek)
            {
                listPaySheetWorkingPeriodDtos.Insert(0, GenerateWorkingPeriodByTwiceWeekly((byte)paysheetStartDate.Value.DayOfWeek,
                    paysheetStartDate.Value, 100, paysheetStartDate.Value.DayOfWeek));
            }
            return listPaySheetWorkingPeriodDtos;
        }

        private List<PaySheetWorkingPeriodDto> GenerateWorkingPeriodsByTwiceAMonth(int firstStartDateOfTwiceAMonth, int secondStartDateOfTwiceAMonth,
            DateTime? paysheetStartDate, DateTime? paysheetEndDate)
        {
            var now = DateTime.Now;
            var nextMonth = now.AddMonths(1);
            var paySheetWorkingPeriodDtos = new List<PaySheetWorkingPeriodDto>();
            var id = 1;
            for (var i = 0; i < Period; i++)
            {
                var currentPeriodTime = nextMonth.AddMonths(-i);
                var daysInMonth = DateTime.DaysInMonth(currentPeriodTime.Year, currentPeriodTime.Month);
                // Kỳ làm việc đầu
                var paySheetWorkingPeriodDto = GenerateWorkingPeriodByTwiceAMonth(firstStartDateOfTwiceAMonth, secondStartDateOfTwiceAMonth, daysInMonth, currentPeriodTime, id);
                if (paySheetWorkingPeriodDtos.Count == 20)
                    break;
                paySheetWorkingPeriodDtos.Add(paySheetWorkingPeriodDto);
                id++;

                if (firstStartDateOfTwiceAMonth > daysInMonth)
                {
                    continue;
                }
                // Kỳ làm việc thứ hai
                paySheetWorkingPeriodDto = GenerateWorkingPeriodByTwiceAMonth(secondStartDateOfTwiceAMonth, firstStartDateOfTwiceAMonth, daysInMonth, currentPeriodTime, id);
                paySheetWorkingPeriodDtos.Add(paySheetWorkingPeriodDto);
                id++;
            }

            paySheetWorkingPeriodDtos = paySheetWorkingPeriodDtos.OrderByDescending(p => p.StartTime).ToList();
            if (paysheetStartDate != null && paysheetEndDate != null
                && !paySheetWorkingPeriodDtos.Any(p => p.StartTime == paysheetStartDate.Value.Date && p.EndTime == paysheetEndDate.Value.Date.AddDays(1).AddSeconds(-1)))
            {
                var daysInMonth = DateTime.DaysInMonth(paysheetStartDate.Value.Year, paysheetStartDate.Value.Month);
                paySheetWorkingPeriodDtos.Insert(0,
                    GenerateWorkingPeriodByTwiceAMonth(paysheetStartDate.Value.Day,
                        paysheetEndDate.Value.AddDays(1).Day, daysInMonth, paysheetStartDate.Value, 100));
            }
            return paySheetWorkingPeriodDtos;
        }

        private PaySheetWorkingPeriodDto GenerateWorkingPeriodByEveryMonth(int startDateOfEveryMonth, DateTime currentPeriodTime, int index)
        {
            var numberDaysInMonth = DateTime.DaysInMonth(currentPeriodTime.Year, currentPeriodTime.Month);
            var startDate = startDateOfEveryMonth > numberDaysInMonth
                ? new DateTime(currentPeriodTime.Year, currentPeriodTime.Month, numberDaysInMonth).AddDays(1)
                : new DateTime(currentPeriodTime.Year, currentPeriodTime.Month, startDateOfEveryMonth);

            var nextMonth = startDateOfEveryMonth > numberDaysInMonth ? startDate : startDate.AddMonths(1);
            var numberDaysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
            var dayOfEndDate = startDateOfEveryMonth > numberDaysInNextMonth
                ? numberDaysInNextMonth
                : startDateOfEveryMonth;
            var endDate = new DateTime(nextMonth.Year, nextMonth.Month, dayOfEndDate);
            endDate = startDateOfEveryMonth > numberDaysInNextMonth ? endDate.AddDays(1).AddSeconds(-1) : endDate.AddSeconds(-1);

            return new PaySheetWorkingPeriodDto
            {
                Id = index + 1,
                Name = startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"),
                StartTime = startDate,
                EndTime = endDate
            };
        }

        private PaySheetWorkingPeriodDto GenerateWorkingPeriodByTwiceAMonth(int startDateOfWorkingPeriod, int endDateOfWorkingPeriod, int daysInMonth,
            DateTime currentPeriodTime, int index)
        {
            var startDate = startDateOfWorkingPeriod > daysInMonth
                ? new DateTime(currentPeriodTime.Year, currentPeriodTime.Month, daysInMonth).AddDays(1)
                : new DateTime(currentPeriodTime.Year, currentPeriodTime.Month, startDateOfWorkingPeriod);

            var nextMonth = endDateOfWorkingPeriod < startDateOfWorkingPeriod && startDateOfWorkingPeriod <= daysInMonth
                ? startDate.AddMonths(1)
                : startDate;
            var numberDaysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
            var dayOfEndDate = endDateOfWorkingPeriod > numberDaysInNextMonth
                ? numberDaysInNextMonth
                : endDateOfWorkingPeriod;
            var endDate = new DateTime(nextMonth.Year, nextMonth.Month, dayOfEndDate);
            endDate = endDateOfWorkingPeriod > numberDaysInNextMonth ? endDate.AddDays(1).AddSeconds(-1) : endDate.AddSeconds(-1);

            return new PaySheetWorkingPeriodDto
            {
                Id = index,
                Name = startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"),
                StartTime = startDate,
                EndTime = endDate
            };
        }

        private PaySheetWorkingPeriodDto GenerateWorkingPeriodByEveryWeek(int startDayOfWeekEveryWeek, DateTime currentPeriodTime, int index, DayOfWeek currentDayOfWeek)
        {
            var startDate = currentPeriodTime.AddDays(startDayOfWeekEveryWeek - (byte)currentDayOfWeek);
            var endDate = startDate.AddDays(7).Date.AddSeconds(-1);

            return new PaySheetWorkingPeriodDto
            {
                Id = index + 1,
                Name = startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy"),
                StartTime = startDate,
                EndTime = endDate
            };
        }

    }
}
