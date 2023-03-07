using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerMachineAggregate.Specifications
{
    public class FindFingerMachineByMachineIdSpec : ExpressionSpecification<FingerMachine>
    {
        public FindFingerMachineByMachineIdSpec(string machineId)
            : base(x => x.MachineId == machineId)
        {
        }
    }
}
