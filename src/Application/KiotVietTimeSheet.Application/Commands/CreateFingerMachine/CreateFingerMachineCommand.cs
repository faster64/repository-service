using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CreateFingerMachine
{
    public class CreateFingerMachineCommand : BaseCommand<FingerMachineDto>
    {
        public FingerMachineDto FingerMachine { get; set; }

        public CreateFingerMachineCommand(FingerMachineDto fingerMachine)
        {
            FingerMachine = fingerMachine;
        }
    }
}
