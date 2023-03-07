using System.Threading.Tasks;
using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Validators;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.ShiftValidators
{
    public class CreateOrUpdateShiftAsyncValidator : ShiftValidator<Shift>
    {
        #region PROPERTIES

        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;

        #endregion

        #region CONSTRUCTOR

        public CreateOrUpdateShiftAsyncValidator(
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            Shift shift
        )
        {
            _shiftReadOnlyRepository = shiftReadOnlyRepository;

            ValidateName();
            ValidateFrom();
            ValidateTo();

            ValidateExistName(shift);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Thực hiện validate form
        /// </summary>
        /// <param name="shift"></param>
        protected void ValidateExistName(Shift shift)
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) => await ShiftNameIsAlreadyExistsInBranch(shift))
                .WithMessage(string.Format(Message.is_exists, Label.shift_name));
        }

        /// <summary>
        /// Kiểm tra tên ca đã tồn tại hay chưa
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        private async Task<bool> ShiftNameIsAlreadyExistsInBranch(Shift shift)
        {
            if (string.IsNullOrEmpty(shift.Name)) return true;
            return !(await _shiftReadOnlyRepository.AnyBySpecificationAsync(
                       new FindShiftByBranchIdSpec(shift.BranchId).Not(new FindShiftByIdSpec(shift.Id)).And(new FindShiftByNameSpec(shift.Name))));
        }

        #endregion
    }
}
