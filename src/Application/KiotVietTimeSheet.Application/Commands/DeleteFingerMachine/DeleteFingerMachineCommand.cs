using KiotVietTimeSheet.Application.Abstractions;

namespace KiotVietTimeSheet.Application.Commands.DeleteFingerMachine
{
    public class DeleteFingerMachineCommand : BaseCommand
    {
        public long Id { get; set; }

        public DeleteFingerMachineCommand(long id)
        {
            Id = id;
        }
    }
}
