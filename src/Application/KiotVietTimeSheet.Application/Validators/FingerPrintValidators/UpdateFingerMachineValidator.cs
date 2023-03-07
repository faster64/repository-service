using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Validators;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.FingerPrintValidators
{
    public class UpdateFingerMachineValidator : FingerMachineValidator<FingerMachine>
    {
        private readonly IFingerMachineReadOnlyRepository _fingerMachineReadOnlyRepository;

        public UpdateFingerMachineValidator(
            IFingerMachineReadOnlyRepository fingerMachineReadOnlyRepository,
            FingerMachine fingerMachine
            )
        {
            _fingerMachineReadOnlyRepository = fingerMachineReadOnlyRepository;
            ValidateName();
            ValidateMachineId();
            ValidateExistNameWhenUpdate(fingerMachine);
        }

        private void ValidateExistNameWhenUpdate(FingerMachine machine)
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) => !await _fingerMachineReadOnlyRepository.MachineNameIsAlreadyExistsInBranchAsync(machine))
                .WithMessage(string.Format(Message.is_exists, Label.fingerMachine_name));
        }
    }
}
