using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.ConfigModels;
using KiotVietTimeSheet.Utilities;
using ServiceStack;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Validators
{
    public class BaseTimeSheetValidator<T> : AbstractValidator<T> where T : List<TimeSheet>
    {
        public BaseTimeSheetValidator() { }

        protected void ValidateId()
        {
            RuleForEach(c => c).Cascade(CascadeMode.StopOnFirstFailure).Must(timeSheet => timeSheet.Id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateStartDate()
        {
            RuleForEach(c => c).Cascade(CascadeMode.StopOnFirstFailure)
                .Must(c => c.StartDate != default(DateTime))
                .WithMessage(string.Format(Message.not_empty, Label.startDate));
        }

        protected void ValidateEndDate(TimeSheetValidateConfiguration timeSheetValidateConfiguration)
        {
            RuleForEach(c => c).Cascade(CascadeMode.StopOnFirstFailure)
                .Where(c => c.AutoGenerateClockingStatus != (byte)AutoGenerateClockingStatus.Auto)
                .Must(c => c.EndDate != default(DateTime))
                .WithMessage(string.Format(Message.not_empty, Label.endDate))
                .Must(c => c.EndDate >= c.StartDate)
                .WithMessage(Message.timeSheet_lessthanStartDateEndDate)
                .Must(c => c.EndDate.Subtract(c.StartDate).Days <= timeSheetValidateConfiguration.AllowOrderMaxDay)
                .WithMessage(Message.timeSheet_maximumBookCalendar.Fmt(timeSheetValidateConfiguration.AllowOrderMaxMonth));
        }

        protected void ValidateRepeatDaysOfWeek()
        {
            RuleForEach(c => c).Cascade(CascadeMode.StopOnFirstFailure)
                .Where(c => c.RepeatType.Equals((byte)RepeatTypes.Weekly))
                .Must(c => c.TimeSheetShifts.All(x => !string.IsNullOrEmpty(x.RepeatDaysOfWeek)))
                .WithMessage(string.Format(Message.not_empty, Label.secondIteration));

        }

        protected void ValidateEmployeeId()
        {
            RuleForEach(t => t).Cascade(CascadeMode.StopOnFirstFailure)
                .Must(c => c.EmployeeId != 0)
                .WithMessage(Message.timeSheet_employeesNotExist);
        }

        protected void ValidateRepeatTimeSheet()
        {
            RuleForEach(t => t).Cascade(CascadeMode.StopOnFirstFailure)
                .Must(c => (c.IsRepeat == null || c.IsRepeat == false) || (c.IsRepeat.Value && c.StartDate.Date < c.EndDate.Date))
                .WithMessage(Message.timeSheet_greaterStartDateEndDate);
        }
    }
}
