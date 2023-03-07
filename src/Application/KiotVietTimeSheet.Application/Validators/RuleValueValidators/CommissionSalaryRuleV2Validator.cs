using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.RuleValueValidators
{
    public class CommissionSalaryRuleV2Validator : CommissionSalaryRuleValueValidatorV2
    {
        private readonly Employee _sourceEmployee;
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        private readonly ICommissionBranchReadOnlyRepository _commissionBranchReadOnlyRepository;
        public CommissionSalaryRuleV2Validator(
            Employee sourceEmployee,
            ICommissionReadOnlyRepository commissionReadOnlyRepository,
            ICommissionBranchReadOnlyRepository commissionBranchReadOnlyRepository
            )
        {
            _sourceEmployee = sourceEmployee;
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            _commissionBranchReadOnlyRepository = commissionBranchReadOnlyRepository;
            ValidateCommissionIsDeletedOrDeActiveAsync();
        }
        protected void ValidateCommissionIsDeletedOrDeActiveAsync()
        {
            RuleForEach(e => e.CommissionSalaryRuleValueDetails)
                .CustomAsync(async (commissionSalaryDetail, context, token) =>
                {
                    if (commissionSalaryDetail.CommissionTableId != null)
                    {
                        var commission = await _commissionReadOnlyRepository.FindByIdAsync(commissionSalaryDetail.CommissionTableId, false, true);
                        if (commission != null)
                        {
                            //Kiểm tra bảng hoa hồng  đã xóa và ngừng áp dụng
                            AddFailureCommission(commission, context);

                            //Kiểm tra bảng hoa hồng còn áp dụng chi nhánh
                            await AddFailureApplyInBranch(commission, context);
                        }
                    }
                });
        }

        private void AddFailureCommission(Commission commission, CustomContext context)
        {
            if (commission.IsDeleted || !commission.IsActive)
            {
                context.AddFailure(commission.IsDeleted
                    ? string.Format(Message.is_deleted, Label.commission_table, commission.Name.Substring(0, commission.Name.Length - 5))
                    : string.Format(Message.is_stopApply, commission.Name, string.Empty));
            }
        }

        private async Task AddFailureApplyInBranch(Commission commission, CustomContext context)
        {
            if (_sourceEmployee == null) return;
            
            if (commission.IsAllBranch) return;

            var commissionBranch = await _commissionBranchReadOnlyRepository.FindBySpecificationAsync(
                new FindCommissionBranchByBranchIdSpec(_sourceEmployee.BranchId).And(new FindCommissionBranchByCommissionIdSpec(commission.Id)));

            if (commissionBranch == null)
            {
                context.AddFailure(string.Format(Message.commission_InActiveForBranch, commission.Name));
            }
        }
    }
}