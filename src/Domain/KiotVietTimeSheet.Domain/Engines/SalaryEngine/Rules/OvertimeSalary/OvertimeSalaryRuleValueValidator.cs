using FluentValidation;
using KiotVietTimeSheet.Resources;
using ServiceStack;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryRuleValueValidator : AbstractValidator<OvertimeSalaryRuleValue>
    {
        protected void ValidateRuleValue()
        {
            RuleFor(e => e)
                .Custom((rule, context) =>
                {
                    if (rule.OvertimeSalaryDays == null || rule.OvertimeSalaryDays.Count == 0)
                    {
                        context.AddFailure(Message.not_haveSelectedOvertime);
                        return;
                    }
                    
                    var isApply = false;
                    foreach (var day in rule.OvertimeSalaryDays)
                    {
                        if (day.Value < 0 && day.IsApply)
                        {
                            context.AddFailure(string.Format(Message.not_haveSelectedOvertimeSalaryWithDay), day.Type.ToDescription());
                            return;
                        }
                        
                        if (day.IsApply) isApply = true;
                    }

                    if (!isApply) context.AddFailure(Message.not_haveSelectedOvertime);
                });
        }
    }
}
