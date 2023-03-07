using System;
using System.Collections.Generic;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Validators
{
    public class BaseClockingValidator<T> : AbstractValidator<T> where T : Clocking
    {
        public BaseClockingValidator() { }
        protected void ValidateId()
        {
            RuleFor(c => c.Id).Must(id => id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateNote()
        {
            When(c => !string.IsNullOrEmpty(c.Note), () =>
            {
                RuleFor(c => c.Note)
                    .Must(note => note.Length <= 250)
                    .WithMessage(string.Format(Message.not_lessThan, Label.note, $"250"));
            });
        }
        protected void ValidateTime()
        {
            RuleFor(c => c)
                .Must(c => c.StartTime != default(DateTime) &&  c.EndTime != default(DateTime))
                .WithMessage(string.Format(Message.not_invalid, Label.time));
        }
        protected void ValidateSameWorkingTime(List<Clocking> clockings)
        {
            RuleFor(c => c)
                .Must(c => clockings.TrueForAll(item => item.StartTime.TimeOfDay == c.StartTime.TimeOfDay && item.EndTime.TimeOfDay == c.EndTime.TimeOfDay))
                .WithMessage(Message.clocking_sameWorkingTime);
        }
        protected void ValidateSameShift(List<Clocking> clockings)
        {
            RuleFor(c => c)
                .Must(c => clockings.TrueForAll(item => item.ShiftId == c.ShiftId))
                .WithMessage(Message.clocking_sameShift);
        }
        protected void ValidateStartTimeBigerThanToday(List<Clocking> clockings)
        {
            RuleFor(c => c)
                .Must(c => clockings.TrueForAll(item => item.StartTime.Date <= DateTime.Now.Date))
                .WithMessage(Message.clocking_greaterStartTimeToday);
        }

        protected void ValidateMaximumSelectedClocking(List<Clocking> clockings)
        {
            RuleFor(c => c)
                .Must(t => clockings.Count <= 100)
                .WithMessage(Message.clocking_maximumSelectedClocking);
        }
    }
}
