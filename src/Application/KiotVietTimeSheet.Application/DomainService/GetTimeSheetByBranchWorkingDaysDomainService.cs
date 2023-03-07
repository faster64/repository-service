using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using static System.Linq.Enumerable;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class GetTimeSheetByBranchWorkingDaysDomainService : IGetTimeSheetByBranchWorkingDaysDomainService
    {
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        public GetTimeSheetByBranchWorkingDaysDomainService(
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository

        )
        {
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
        }
        public List<TimeSheet> GetTimeSheetByBranchWorkingDay(GetTimeSheetByBranchWorkingDaysDto getTimeSheetByBranchWorkingDaysDto)
        {
            if (getTimeSheetByBranchWorkingDaysDto.WorkingDays == null || !getTimeSheetByBranchWorkingDaysDto.WorkingDays.Any())
            {
                return new List<TimeSheet>();
            }
            var timeSheets = _timeSheetWriteOnlyRepository.GetTimeSheetForBranchSetting(getTimeSheetByBranchWorkingDaysDto.BranchId, getTimeSheetByBranchWorkingDaysDto.ApplyFrom);

            if (timeSheets == null || !timeSheets.Any())
                return new List<TimeSheet>();

            timeSheets = timeSheets.Where(x => !x.SaveOnDaysOffOfBranch).ToList();

            if (timeSheets.Any())
            {
                timeSheets = timeSheets.OrderByDescending(x => x.EndDate).ToList();
                var applyFrom = getTimeSheetByBranchWorkingDaysDto.ApplyFrom;
                var applyTo = timeSheets.First().EndDate;
                var dayOfWeeks = new List<DayOfWeek>();
                foreach (var day in getTimeSheetByBranchWorkingDaysDto.WorkingDays)
                {
                    dayOfWeeks.Add((DayOfWeek)day);
                }
                var range = Range(0, (applyTo - applyFrom).Days + 1)
                    .Select(d => applyFrom.AddDays(d))
                    .Where(dt => dayOfWeeks.Contains(dt.DayOfWeek)).ToList();
                if (range.Any())
                {
                    timeSheets = timeSheets.Where(x => range.Any(y => x.StartDate.Date <= y && y <= applyTo)).ToList();
                }
                return timeSheets;
            }
            else
            {
                return new List<TimeSheet>();
            }

        }
    }
}
