using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Validators
{
    public class FingerMachineValidator<T> : AbstractValidator<T> where T : FingerMachine
    {
        public FingerMachineValidator()
        {
        }

        protected void ValidateName()
        {
            RuleFor(c => c.MachineName)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .WithMessage(Message.fingerMachine_emptyName);
        }

        protected void ValidateMachineId()
        {
            RuleFor(c => c.MachineId)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .WithMessage(Message.fingerMachine_emptySerial);
        }
    }
}
