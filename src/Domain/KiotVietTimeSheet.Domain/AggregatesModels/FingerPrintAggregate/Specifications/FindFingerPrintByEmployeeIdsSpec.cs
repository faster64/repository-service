using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications
{
    public class FindFingerPrintByEmployeeIdsSpec : ExpressionSpecification<FingerPrint>
    {
        public FindFingerPrintByEmployeeIdsSpec(List<long> employeeIds)
            : base(x => employeeIds.Contains(x.EmployeeId))
        {
        }
    }
}
