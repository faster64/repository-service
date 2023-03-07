using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Validations
{
    public class BaseJobTitleValidator<T> : AbstractValidator<T> where T : JobTitle
    {
        protected void ValidateName()
        {
            RuleFor(c => c.Name)
               .Must(c => !string.IsNullOrWhiteSpace(c))
               .WithMessage(string.Format(Message.not_empty, Label.jobTitle_name))
               .MaximumLength(50)
               .WithMessage(string.Format(Message.not_lessThan, Label.jobTitle_name, $"50"));
        }

        protected void ValidateDescription()
        {
            When(c => !string.IsNullOrWhiteSpace(c.Description), () =>
            {
                RuleFor(c => c.Description)
                    .MaximumLength(500)
                    .WithMessage(string.Format(Message.not_lessThan, Label.note, $"500"));
            });
        }
    }
}
