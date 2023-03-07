using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerPrintByEmployeeIdSpec : ExpressionSpecification<FingerPrint>
    {
        public FindFingerPrintByEmployeeIdSpec(long employeeId)
            : base(x => x.EmployeeId == employeeId)
        {
        }
    }
}
