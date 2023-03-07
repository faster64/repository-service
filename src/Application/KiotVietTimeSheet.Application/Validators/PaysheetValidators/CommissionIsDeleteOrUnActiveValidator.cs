using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Specifications;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PaysheetValidators
{
    public class CommissionIsDeleteOrUnActiveValidator : CommissionSalaryRuleParamValidatorV2
    {
        private readonly ICommissionReadOnlyRepository _commissionReadOnlyRepository;
        public CommissionIsDeleteOrUnActiveValidator(ICommissionReadOnlyRepository commissionReadOnlyRepository)
        {
            _commissionReadOnlyRepository = commissionReadOnlyRepository;
            ValidateCommissionIsDeleteOrUnActive();
        }

        protected void ValidateCommissionIsDeleteOrUnActive()
        {
            RuleFor(p => p)
                .CustomAsync(async (commissionSalaryParam, context, token) =>
                {
                    var commissionParams = 
                        commissionSalaryParam.CommissionParams
                                             .Where(x => x.CommissionTable != null)
                                             .ToList();

                    var commissionIds = commissionParams.Select(x => x.CommissionTable.Id).ToList();
                    
                    var commissionFromSource = await 
                        _commissionReadOnlyRepository.GetBySpecificationAsync(
                            new FindCommissionByIdsSpec(commissionIds), false, true);

                    foreach (var item in commissionParams)
                    {
                        var commission = commissionFromSource.FirstOrDefault(x => x.Id == item.CommissionTable.Id);

                        if (commission == null || (commission.IsActive && !commission.IsDeleted)) continue;

                        AddFailureCommissionDelete(commission, context);

                        break;
                    }
                });
        }

        private void AddFailureCommissionDelete(Commission commission, CustomContext context)
        {
            context.AddFailure(commission.IsDeleted
                ? string.Format(Message.is_deletedCheckAgain, Label.commission_table, commission.Name.Substring(0, commission.Name.Length - 5))
                : string.Format(Message.is_stopApplyCheckAgain, Label.commission_table, commission.Name));
        }
    }
}
