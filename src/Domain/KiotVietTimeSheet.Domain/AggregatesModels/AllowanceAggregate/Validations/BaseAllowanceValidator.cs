using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Validations
{
    public class BaseAllowanceValidator<T> : AbstractValidator<T> where T : Allowance
    {
        protected void ValidateName()
        {
            RuleFor(c => c.Name)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.allowance_name))
               .MaximumLength(50)
               .WithMessage(string.Format(Message.not_lessThan, Label.allowance_name, $"50"));
        }
    }
}
