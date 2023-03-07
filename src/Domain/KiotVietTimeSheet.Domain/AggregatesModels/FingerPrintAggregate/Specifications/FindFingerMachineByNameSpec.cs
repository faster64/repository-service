using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerMachineByNameSpec : ExpressionSpecification<FingerMachine>
    {
        public FindFingerMachineByNameSpec(string machineName)
            : base(x => x.MachineName == machineName)
        {
        }
    }
}
