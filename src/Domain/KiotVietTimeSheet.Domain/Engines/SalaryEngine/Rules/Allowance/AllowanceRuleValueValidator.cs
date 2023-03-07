using FluentValidation;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public class AllowanceRuleValueValidator : AbstractValidator<AllowanceRuleValue>
    {

        protected void ValidateAllowance()
        {
            RuleForEach(e => e.AllowanceRuleValueDetails)
                .Must((rule, detail) => detail.AllowanceId > 0)
                .WithMessage("Bạn chưa chọn phụ cấp.");
        }

        protected void ValidateRuleValue()
        {
            RuleForEach(e => e.AllowanceRuleValueDetails)
                .Must((rule, detail) => detail.ValueRatio != null || detail.Value != null)
                .WithMessage("Bạn chưa nhập giá trị phụ cấp cho nhân viên.");
        }
    }
}
