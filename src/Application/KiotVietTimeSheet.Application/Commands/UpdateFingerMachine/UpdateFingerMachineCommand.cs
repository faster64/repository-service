using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.UpdateFingerMachine
{
    public class UpdateFingerMachineCommand : BaseCommand<FingerMachineDto>
    {
        public FingerMachineDto FingerMachine { get; set; }

        public UpdateFingerMachineCommand(FingerMachineDto fingerMachine)
        {
            FingerMachine = fingerMachine;
        }
    }
}
