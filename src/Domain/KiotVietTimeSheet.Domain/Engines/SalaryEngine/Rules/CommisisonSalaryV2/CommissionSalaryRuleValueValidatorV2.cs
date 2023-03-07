using FluentValidation;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public class CommissionSalaryRuleValueValidatorV2 : AbstractValidator<CommissionSalaryRuleValueV2>
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
                .Must(e => e.ValueRatio != null || e.Value != null || e.CommissionTableId.GetValueOrDefault() > 0)
                .WithMessage("Bạn chưa nhập hệ số hoa hồng.");
        }
    }
}
