using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using System;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Validators
{
    public abstract class HolidayValidator<T> : AbstractValidator<T> where T : Holiday
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id).Must(id => id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .WithMessage(Message.holiday_emptyName)
                .MaximumLength(100)
                .WithMessage(string.Format(Message.not_lessThan, Label.holiday_name, $"100"));
        }

        protected void ValidateFrom()
        {
            RuleFor(c => c)
                .Must(t => t.From != default(DateTime))
                .WithMessage(Message.holiday_emptyFrom)
                .Must(t => t.From.Date <= t.To.Date)
                .WithMessage(Message.holiday_fromCannotGreaterThanTo);
        }

        protected void ValidateTo()
        {
            RuleFor(c => c.To)
                .Must(t => t != default(DateTime))
                .WithMessage(Message.holiday_emptyTo)
                .GreaterThanOrEqualTo(c => c.From)
                .WithMessage(Message.holiday_toMustGreaterOrEqualThanFrom)
                .Must((c, e) => e.Subtract(c.From).Days < 31)
                .WithMessage(Message.holiday_maximumDays);
        }
    }
}
