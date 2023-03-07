using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Validations;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.PayRateValidators
{
    public class PenalizeCreateOrUpdateValidator : BasePenalizeValidator<Penalize>
    {
        #region Properties
        private readonly IPenalizeReadOnlyRepository _penalizeReadOnlyRepository;
        #endregion
        public PenalizeCreateOrUpdateValidator(IPenalizeReadOnlyRepository penalizeReadOnlyRepository)
        {
            _penalizeReadOnlyRepository = penalizeReadOnlyRepository;

            ValidateName();
            ValidatePenalizeIsExistsByNameAsync();
        }

        #region Protected methods
        protected void ValidatePenalizeIsExistsByNameAsync()
        {
            RuleFor(e => e)
                .MustAsync(async (penalize, token) =>
                {
                    var spec = (new FindPenalizeByNameSpec(penalize.Name)).Not(new FindPenalizeByIdSpec(penalize.Id));
                    var existingPenalize = await _penalizeReadOnlyRepository.FindBySpecificationAsync(spec);

                    if (existingPenalize != null)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(string.Format(Message.is_existsInSystem, Label.penalize_name));
        }
        #endregion
    }
}
