using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Validators
{
    public class CreateOrUpdateMachineValidator : FingerMachineValidator<FingerMachine>
    {
        public CreateOrUpdateMachineValidator()
        {
            ValidateName();
            ValidateMachineId();
        }
    }
}
