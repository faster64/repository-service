using FluentValidation;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public class DeductionRuleValueValidator : AbstractValidator<DeductionRuleValue>
    {
        protected void ValidateDeduction()
        {
            RuleForEach(e => e.DeductionRuleValueDetails)
                .Must((rule, detail) => detail.DeductionId > 0)
                .WithMessage("Bạn chưa chọn giảm trừ.");
        }

        protected void ValidateRuleValue()
        {
            RuleForEach(e => e.DeductionRuleValueDetails)
                .Must((rule, detail) => detail.ValueRatio != null || detail.Value != null)
                .WithMessage("Bạn chưa nhập giá trị giảm trừ cho nhân viên.");
        }
    }
}
