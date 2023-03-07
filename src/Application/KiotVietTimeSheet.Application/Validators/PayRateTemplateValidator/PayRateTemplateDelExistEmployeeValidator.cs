using FluentValidation;
using FluentValidation.Results;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Validations;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateTemplateValidator
{
    public class PayRateTemplateDelExistEmployeeValidator : BasePayRateTemplateValidator<PayRateTemplate>
    {
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        public PayRateTemplateDelExistEmployeeValidator(IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository)
        {
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;

            ValidateExistUsed();
        }

        private void ValidateExistUsed()
        {
            RuleFor(e => e)
               .CustomAsync(async (payRateTemplate, context, token) =>
               {
                   var exist = await _payRateTemplateReadOnlyRepository.ExistEmployeeDataAsync(payRateTemplate.Id);
                   if (exist)
                   {
                       context.AddFailure(string.Format(Message.validate_when_del_payrateTpl));
                   }
               });
        }
    }
}
