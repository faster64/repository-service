using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Validations
{
    public class BaseDepartmentValidator<T> : AbstractValidator<T> where T : Department
    {
        protected void ValidateName()
        {
            RuleFor(c => c.Name)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.department_name))
               .MaximumLength(50)
               .WithMessage(string.Format(Message.not_lessThan, Label.deduction_name, $"50"));
        }

        protected void ValidateDescription()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Description), () =>
            {
                RuleFor(c => c.Description)
                    .MaximumLength(500)
                    .WithMessage(string.Format(Message.not_lessThan, Label.description, $"500"));
            });
        }
    }
}
