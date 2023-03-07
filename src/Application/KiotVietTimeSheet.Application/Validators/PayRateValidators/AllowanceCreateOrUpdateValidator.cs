using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Validations;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateValidators
{
    public class AllowanceCreateOrUpdateValidator : BaseAllowanceValidator<Allowance>
    {
        #region Properties
        private readonly IAllowanceReadOnlyRepository _allowanceRepository;
        #endregion
        public AllowanceCreateOrUpdateValidator(IAllowanceReadOnlyRepository allowanceRepository)
        {
            _allowanceRepository = allowanceRepository;

            ValidateName();
            ValidateAllowanceIsExistsByNameAsync();
        }

        #region Protected methods
        protected void ValidateAllowanceIsExistsByNameAsync()
        {
            RuleFor(e => e)
                .MustAsync(async (allowance, token) =>
                {
                    var spec = (new FindAllowanceByNameSpec(allowance.Name)).Not(new FindAllowanceByIdSpec(allowance.Id));
                    var existingAllowance = await _allowanceRepository.FindBySpecificationAsync(spec);

                    if (existingAllowance != null)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(string.Format(Message.is_existsInSystem, Label.allowance_name));
        }
        #endregion
    }
}
