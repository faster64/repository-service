using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;

namespace KiotVietTimeSheet.Application.Commands.CreateDeduction
{
    [RequiredPermission(TimeSheetPermission.Deduction_Create)]
    public class CreateDeductionCommand : BaseCommand<DeductionDto>
    {
        public DeductionDto Deduction { get; }
        public DeductionRuleValueDetail DeductionDetail { get; }

        public CreateDeductionCommand(DeductionDto deduction, DeductionRuleValueDetail deductionDetail)
        {
            Deduction = deduction;
            DeductionDetail = deductionDetail;
        }
    }
}