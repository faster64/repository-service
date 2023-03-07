using FluentValidation;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Validators;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Application.Validators.FingerPrintValidators
{
    public class CreateFingerMachineValidator : FingerMachineValidator<FingerMachine>
    {
        private readonly IFingerMachineReadOnlyRepository _fingerMachineReadOnlyRepository;

        public CreateFingerMachineValidator(
            IFingerMachineReadOnlyRepository fingerMachineReadOnlyRepository,
            FingerMachine fingerMachine)
        {
            _fingerMachineReadOnlyRepository = fingerMachineReadOnlyRepository;
            ValidateName();
            ValidateExistNameWhenCreate(fingerMachine);
            ValidateMachineId();
            ValidatorExistMachineId(fingerMachine);
        }

        private void ValidatorExistMachineId(FingerMachine machine)
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) => !await _fingerMachineReadOnlyRepository.MachineIdIsAlreadyExistsInBranchAsync(machine))
                .WithMessage(string.Format(Message.is_exists, Label.fingerMachine_serial));
        }

        private void ValidateExistNameWhenCreate(FingerMachine machine)
        {
            RuleFor(x => x)
                .MustAsync(async (c, token) => !await _fingerMachineReadOnlyRepository.MachineNameIsAlreadyExistsInBranchAsync(machine))
                .WithMessage(string.Format(Message.is_exists, Label.fingerMachine_name));
        }

    }
}
