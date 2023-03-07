using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerMachineByMachineId : ExpressionSpecification<FingerMachine>
    {
        public FindFingerMachineByMachineId(string machineId)
            : base(x => x.MachineId == machineId)
        {
        }
    }
}
