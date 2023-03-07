using System.Collections.Generic;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Validations
{
    public class BasePayRateTemplateValidator<T> : AbstractValidator<T> where T : PayRateTemplate
    {
        protected readonly List<IRule> _rules;

        public BasePayRateTemplateValidator()
        {

        }

        public BasePayRateTemplateValidator(List<IRule> rules)
        {
            _rules = rules;
        }

        protected void ValidateName()
        {
            RuleFor(c => c.Name)
                .Must(c => !string.IsNullOrWhiteSpace(c))
                .WithMessage(Message.payRateTemplate_emptyName)
                .MaximumLength(50)
                .WithMessage(string.Format(Message.not_lessThan, Label.template_name, $"50"));
        }
    }
}
