using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.RuleValueValidators
{
    public class DeductionRuleValidator : DeductionRuleValueValidator
    {
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        public DeductionRuleValidator(IDeductionReadOnlyRepository deductionReadOnlyRepository)
        {
            _deductionReadOnlyRepository = deductionReadOnlyRepository;

            ValidateDeduction();
            ValidateRuleValue();
            ValidateDeductionNotExistAsync();
        }

        protected void ValidateDeductionNotExistAsync()
        {
            RuleForEach(e => e.DeductionRuleValueDetails)
                .CustomAsync(async (deductionRule, context, token) =>
                {
                    var deductionDeleted = await _deductionReadOnlyRepository.FindByIdAsync(deductionRule.DeductionId, false, true);
                    if (deductionDeleted != null && deductionDeleted.IsDeleted)
                    {
                        context.AddFailure(string.Format(Message.is_deletedCheckAgain, Label.deduction, deductionDeleted.Name.Substring(0, deductionDeleted.Name.Length - 5)));
                    }
                });
        }
    }
}
