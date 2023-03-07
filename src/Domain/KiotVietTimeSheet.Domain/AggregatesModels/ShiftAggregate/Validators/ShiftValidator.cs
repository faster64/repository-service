using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Validators
{
    public abstract class ShiftValidator<T> : AbstractValidator<T> where T : Shift
    {
        protected ShiftValidator() { }

        protected void ValidateId()
        {
            RuleFor(c => c.Id).Must(id => id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .WithMessage(Message.shift_haveNotInputtedName)
                .MaximumLength(50)
                .WithMessage(string.Format(Message.not_lessThan, Label.shift_name, $"50"));
        }

        protected void ValidateFrom()
        {
            RuleFor(c => c.From)
                .NotNull()
                .WithMessage(string.Format(Message.not_empty, Label.startTime))
                .NotEqual(c => c.To)
                .WithMessage(Message.shift_duplicatedFromTimeToTime);
        }

        protected void ValidateTo()
        {
            RuleFor(c => c.To)
                .NotNull()
                .WithMessage(string.Format(Message.not_empty, Label.endTime))
                .NotEqual(c => c.From)
                .WithMessage(Message.shift_duplicatedFromTimeToTime);
        }
    }
}
