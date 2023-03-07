using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Validators.RuleValueValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Validations;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateValidators
{
    public class PayRateCreateOrUpdateValidator : BasePayRateValidator<PayRate>
    {
        private readonly IPayRateTemplateReadOnlyRepository _payRateTemplateReadOnlyRepository;
        private readonly IAllowanceReadOnlyRepository _allowanceReadOnlyRepository;
        private readonly IDeductionReadOnlyRepository _deductionReadOnlyRepository;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly ICommissionBranchReadOnlyRepository _commissionBranchReadOnlyRepository;
        private readonly Employee _sourceEmployee;

        public PayRateCreateOrUpdateValidator(
            List<IRule> rules,
            IPayRateTemplateReadOnlyRepository payRateTemplateReadOnlyRepository,
            IAllowanceReadOnlyRepository allowanceReadOnlyRepository,
            IDeductionReadOnlyRepository deductionReadOnlyRepository,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            ICommissionBranchReadOnlyRepository commissionBranchReadOnlyRepository,
            Employee sourceEmployee,
            List<int> workBranchIds,
            List<Shift> shiftFromMainSalaries
            )
            : base(rules)
        {
            _payRateTemplateReadOnlyRepository = payRateTemplateReadOnlyRepository;
            _allowanceReadOnlyRepository = allowanceReadOnlyRepository;
            _deductionReadOnlyRepository = deductionReadOnlyRepository;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _commissionBranchReadOnlyRepository = commissionBranchReadOnlyRepository;
            _sourceEmployee = sourceEmployee;
            ValidateEmployee();
            ValidateTemplateNotExist();
            ValidateSalaryPeriod();
            ValidateMainRule(workBranchIds, shiftFromMainSalaries);
            ValidateOvertimeRule();
            ValidateCommissionSalaryRuleV2();
            ValidateAllowanceRule();
            ValidateDeductionRule();
        }

        protected void ValidateTemplateNotExist()
        {
            RuleFor(e => e)
                .MustAsync(async (payRate, context, token) =>
                {
                    if (payRate.PayRateTemplateId == null || payRate.PayRateTemplateId == 0) return true;
                    var existTemplate =
                        await _payRateTemplateReadOnlyRepository.FindByIdAsync(payRate.PayRateTemplateId,
                            includeSoftDelete: true);
                    return existTemplate != null;
                })
                .WithMessage(e => string.Format(Message.is_deletedCheckAgain, Label.template, string.Empty));
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
        protected void ValidateCommissionSalaryRuleV2()
        {
            RuleFor(template => template)
                .CustomAsync(async (template, context, token) =>
                {
                    if (!(_rules.FirstOrDefault(r => r.GetType() == typeof(CommissionSalaryRuleV2)) is CommissionSalaryRuleV2 commissionSalaryRuleValueV2)) return;
                    var commissionSalaryValidator = await new CommissionSalaryRuleV2Validator(
                        _sourceEmployee,
                        _commissionReadOnlyRepository,
                        _commissionBranchReadOnlyRepository
                        ).ValidateAsync(commissionSalaryRuleValueV2.GetRuleValue() as CommissionSalaryRuleValueV2, token);
                    if (!commissionSalaryValidator.IsValid) commissionSalaryValidator.Errors.ToList().ForEach(context.AddFailure);
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

        protected void ValidateMainRule(List<int> workBranchIds, List<Shift> shiftFromMainSalaries)
        {
            RuleFor(template => template)
                .Custom((template, context) =>
                {
                    if (!(_rules.FirstOrDefault(r => r.GetType() == typeof(MainSalaryRule)) is MainSalaryRule mainSalaryRule)) return;
                    var mainSalaryRuleValidator = new MainSalaryRuleValidator(workBranchIds, shiftFromMainSalaries).Validate(mainSalaryRule.GetRuleValue() as MainSalaryRuleValue);
                    if (!mainSalaryRuleValidator.IsValid) mainSalaryRuleValidator.Errors.ToList().ForEach(context.AddFailure);
                });
        }

        protected void ValidateOvertimeRule()
        {
            RuleFor(template => template)
                .Custom((template, context) =>
                {
                    if (!(_rules.FirstOrDefault(r => r.GetType() == typeof(OvertimeSalaryRule)) is OvertimeSalaryRule overSalaryRule)) return;
                    var overSalaryRuleValidator = new OvertimeSalaryRuleValidator().Validate(overSalaryRule.GetRuleValue() as OvertimeSalaryRuleValue);
                    if (!overSalaryRuleValidator.IsValid) overSalaryRuleValidator.Errors.ToList().ForEach(context.AddFailure);
                });
        }
    }
}
