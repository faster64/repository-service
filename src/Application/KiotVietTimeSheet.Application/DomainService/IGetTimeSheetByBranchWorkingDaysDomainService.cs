using System.Collections.Generic;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface IGetTimeSheetByBranchWorkingDaysDomainService
    {
        List<TimeSheet> GetTimeSheetByBranchWorkingDay(GetTimeSheetByBranchWorkingDaysDto getTimeSheetByBranchWorkingDaysDto);

    }
}
