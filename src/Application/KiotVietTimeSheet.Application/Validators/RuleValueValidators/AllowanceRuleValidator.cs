using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.RuleValueValidators
{
    public class AllowanceRuleValidator : AllowanceRuleValueValidator
    {
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        public AllowanceRuleValidator(IAllowanceReadOnlyRepository allowanceReadOnlyRepository)
        {
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;

            ValidateAllowance();
            ValidateRuleValue();
            ValidateAllowanceNotExistAsync();
        }

        protected void ValidateAllowanceNotExistAsync()
        {
            RuleForEach(e => e.AllowanceRuleValueDetails)
                .CustomAsync(async (allowanceRule, context, token) =>
                {
                    var allowanceDeleted = await _allowanceReadOnlyRepository.FindByIdAsync(allowanceRule.AllowanceId, false, true);
                    if (allowanceDeleted != null && allowanceDeleted.IsDeleted)
                    {
                        context.AddFailure(string.Format(Message.is_deletedCheckAgain, Label.allowance, allowanceDeleted.Name.Substring(0, allowanceDeleted.Name.Length - 5)));
                    }
                });
        }
    }
}
