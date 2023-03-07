using FluentValidation;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommissionSalary
{
    public class CommissionSalaryRuleValueValidator : AbstractValidator<CommissionSalaryRuleValue>
    {
        protected void ValidateMinCommission()
        {
            RuleFor(e => e)
                .Must(e => !e.UseMinCommission || e.UseMinCommission && e.MinCommission != null)
                .WithMessage("Bạn chưa nhập doanh thu tối thiểu");
        }

        protected void ValidateDetails()
        {
            RuleForEach(e => e.CommissionSalaryRuleValueDetails)
                .Must(e => e.ValueRatio != null || e.Value != null)
                .WithMessage("Bạn hệ số lương kinh doanh.");
        }
    }
}
