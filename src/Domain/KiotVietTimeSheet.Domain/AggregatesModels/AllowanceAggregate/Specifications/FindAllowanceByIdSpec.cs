using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications
{
    public class FindAllowanceByIdSpec : ExpressionSpecification<Allowance>
    {
        public FindAllowanceByIdSpec(long id)
            : base(e => e.Id == id)
        { }
    }
}
