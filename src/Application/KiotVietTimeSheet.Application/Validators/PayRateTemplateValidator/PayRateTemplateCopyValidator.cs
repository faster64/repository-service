using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Validations;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateTemplateValidator
{
    public class PayRateTemplateCopyValidator : BasePayRateTemplateValidator<PayRateTemplate>
    {
        public PayRateTemplateCopyValidator()
        {
            ValidateExistPayRateTemplate();
        }

        private void ValidateExistPayRateTemplate()
        {
            RuleFor(e => e)
                .Must((e, token) => e != null)
                .WithMessage(string.Format(Message.not_found, Label.paysheet_template.ToLower()));
        }
    }
}
