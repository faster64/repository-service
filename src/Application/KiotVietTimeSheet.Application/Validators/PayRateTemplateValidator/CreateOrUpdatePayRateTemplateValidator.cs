using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.RuleValueValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Validations;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateTemplateValidator
{
    public class CreateOrUpdatePayRateTemplateValidator : BasePayRateTemplateValidator<PayRateTemplate>
    {
        private readonly IPayRateTemplateWriteOnlyRepository _payRateTemplateWriteOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;

        public CreateOrUpdatePayRateTemplateValidator(List<IRule> rules,
            IPayRateTemplateWriteOnlyRepository payRateTemplateWriteOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            List<Shift> shiftFromMainSalaries)
            : base(rules)
        {
            _payRateTemplateWriteOnlyRepository = payRateTemplateWriteOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;

            ValidateName();
            ValidateExistName();
            ValidateMainRule(shiftFromMainSalaries);
            ValidateAllowanceRule();
            ValidateDeductionRule();
        }

        protected void ValidateExistName()
        {
            RuleFor(e => e)
                .CustomAsync(async (template, context, token) =>
                {
                    var existTemplateSameName = await _payRateTemplateWriteOnlyRepository.FindBySpecificationAsync(
                        new FindPayRateTemplateByNameSpec(template.Name)
                            .And(new FindPayRateTemplateByBranchIdSpec(template.BranchId))
                            .Not(new FindByEntityIdLongSpec<PayRateTemplate>(template.Id)));
                    if (existTemplateSameName != null)
                    {
                        context.AddFailure(string.Format(Message.payRateTemplate_existsName, existTemplateSameName.Name));
                    }
                });
        }

        protected void ValidateAllowanceRule()
        {
            RuleFor(template => template)
                .CustomAsync(async (template, context, token) =>
                {
                    if (!(_rules.FirstOrDefault(r => r.GetType() == typeof(AllowanceRule)) is AllowanceRule allowanceRule)) return;
                    var allowanceValidator = await new AllowanceRuleValidator(_allowanceReadOnlyRepository).ValidateAsync(allowanceRule.GetRuleValue() as AllowanceRuleValue, token);
                    if (!allowanceValidator.IsValid) allowanceValidator.Errors.ToList().ForEach(context.AddFailure);
                });
        }

        protected void ValidateDeductionRule()
        {
            RuleFor(template => template)
                .CustomAsync(async (template, context, token) =>
                {
                    if (!(_rules.FirstOrDefault(r => r.GetType() == typeof(DeductionRule)) is DeductionRule deductionRule)) return;
                    var deductionValidator = await new DeductionRuleValidator(_deductionReadOnlyRepository).ValidateAsync(deductionRule.GetRuleValue() as DeductionRuleValue, token);
                    if (!deductionValidator.IsValid) deductionValidator.Errors.ToList().ForEach(context.AddFailure);
                });
        }

        protected void ValidateMainRule(List<Shift> shiftFromMainSalaries)
        {
            RuleFor(template => template)
                .Custom((template, context) =>
                {
                    if (!(_rules.FirstOrDefault(r => r.GetType() == typeof(MainSalaryRule)) is MainSalaryRule mainSalaryRule)) return;
                    var mainSalaryRuleValidator = new MainSalaryRuleValidator(null, shiftFromMainSalaries).Validate(mainSalaryRule.GetRuleValue() as MainSalaryRuleValue);
                    if (!mainSalaryRuleValidator.IsValid) mainSalaryRuleValidator.Errors.ToList().ForEach(context.AddFailure);
                });
        }
    }
}
