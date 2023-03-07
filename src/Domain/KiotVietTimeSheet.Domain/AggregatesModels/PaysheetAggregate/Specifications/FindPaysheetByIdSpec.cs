using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Specifications
{
    public class FindPaysheetByIdSpec : ExpressionSpecification<Paysheet>
    {
        public FindPaysheetByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
