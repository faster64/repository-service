using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Validations
{
    public class BaseDeductionValidator<T> : AbstractValidator<T> where T : Deduction
    {
        protected void ValidateName()
        {
            RuleFor(c => c.Name)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.deduction_name))
               .MaximumLength(50)
               .WithMessage(string.Format(Message.not_lessThan, Label.deduction_name, $"50"));
        }
    }
}
