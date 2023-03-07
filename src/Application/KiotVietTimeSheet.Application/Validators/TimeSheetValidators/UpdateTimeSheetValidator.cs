using System.Collections.Generic;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.ConfigModels;

namespace KiotVietTimeSheet.Application.Validators.TimeSheetValidators
{
    public class UpdateTimeSheetValidator : OverlapTimeValidator
    {
        public UpdateTimeSheetValidator(
            List<ClockingDto> clockingsOverlapTime,
            List<Clocking> timeSheetClockings,
            TimeSheetValidateConfiguration timeSheetValidateConfiguration
        ) : base(clockingsOverlapTime, timeSheetClockings)
        {
            ValidateStartDate();
            ValidateEndDate(timeSheetValidateConfiguration);
            ValidateRepeatDaysOfWeek();
            ValidateRepeatTimeSheet();
        }
    }
}
