using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByStatusSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByStatusSpec(byte status)
            : base(p => p.PaysheetStatus == status)
        { }
    }
}
