using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.TimeSheetValidators
{
    public class CopyTimeSheetValidator : OverlapTimeValidator
    {
        #region Properties
        #endregion

        #region Constructors
        public CopyTimeSheetValidator(
            List<ClockingDto> clockingsOverlapTime,
            List<TimeSheet> timeSheets,
            List<Clocking> timeSheetClockings,
            DateTime from,
            DateTime to
        ) : base(clockingsOverlapTime, timeSheetClockings)
        {
            ValidateTimeSheetsAmount(timeSheets);
            ValidateMaxTimeCopy(from, to);
        }
        #endregion

        #region Methods
        protected void ValidateTimeSheetsAmount(List<TimeSheet> timeSheets)
        {
            RuleFor(x => x)
                .Must((timeSheet) => timeSheets.Any())
                .WithMessage(Message.timeSheet_noDataInRangeTimeSelectForCopy);
        }

        protected void ValidateMaxTimeCopy(DateTime from, DateTime to)
        {
            RuleFor(x => x)
                .Must((timeSheet) => (to.Subtract(from).Days + 1 <= 31))
                .WithMessage(Message.timeSheet_maxTimeCopy);
        }
        #endregion
    }
}