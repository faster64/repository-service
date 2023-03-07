using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Validators
{
    public class BaseCommissionValidator<T> : AbstractValidator<T> where T : Commission
    {
        public BaseCommissionValidator() { }

        protected void ValidateId()
        {
            RuleFor(c => c.Id).Must(id => id > 0).WithMessage(string.Format(Message.not_invalid, Label.id));
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage(Message.commission_haveNotInputtedName)
                .MaximumLength(255)
                .WithMessage(string.Format(Message.not_lessThan, Label.commission_tableName, $"255"));
        }
    }
}
