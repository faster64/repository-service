using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications
{
    public class FindClockingByAbsenceTypeSpec : ExpressionSpecification<Clocking>
    {
        public FindClockingByAbsenceTypeSpec(byte type)
            : base(c => c.AbsenceType == type)
        { }
    }
}
