using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Auth.Common;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;

namespace KiotVietTimeSheet.Application.Commands.UpdateDeduction
{
    [RequiredPermission(TimeSheetPermission.Deduction_Update)]
    public class UpdateDeductionCommand : BaseCommand<DeductionDto>
    {
        public DeductionDto Deduction { get; }
        public DeductionRuleValueDetail DeductionDetail { get; }

        public UpdateDeductionCommand(DeductionDto deduction, DeductionRuleValueDetail deductionDetail)
        {
            Deduction = deduction;
            DeductionDetail = deductionDetail;
        }
    }
}