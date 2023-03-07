using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Validations;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateValidators
{
    public class DeductionCreateOrUpdateValidator : BaseDeductionValidator<Deduction>
    {
        #region Properties
        private readonly IDeductionReadOnlyRepository _deductionRepository;
        #endregion
        public DeductionCreateOrUpdateValidator(IDeductionReadOnlyRepository deductionRepository)
        {
            _deductionRepository = deductionRepository;

            ValidateName();
            ValidateDeductionIsExistsByNameAsync();
        }

        #region Protected methods
        protected void ValidateDeductionIsExistsByNameAsync()
        {
            RuleFor(e => e)
                .MustAsync(async (deduction, token) =>
                {
                    var spec = (new FindDeductionByNameSpec(deduction.Name)).Not(new FindDeductionByIdSpec(deduction.Id));
                    var existingDeduction = await _deductionRepository.FindBySpecificationAsync(spec);

                    if (existingDeduction != null)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(string.Format(Message.is_existsInSystem, Label.deduction));
        }
        #endregion
    }
}
