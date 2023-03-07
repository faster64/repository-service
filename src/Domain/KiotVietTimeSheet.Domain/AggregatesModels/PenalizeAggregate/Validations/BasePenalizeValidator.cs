using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Validations
{
    public class BasePenalizeValidator<T> : AbstractValidator<T> where T : Penalize
    {
        protected void ValidateName()
        {
            RuleFor(c => c.Name)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.penalize_name))
               .MaximumLength(50)
               .WithMessage(string.Format(Message.not_lessThan, Label.penalize_name, $"255"));
        }
    }
}
