using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Extension;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Specifications
{
    public class FindAllowanceByNameSpec : ExpressionSpecification<Allowance>
    {
        public FindAllowanceByNameSpec(string name)
            : base(e => e.Name.Equals(name.ToPerfectString()))
        { }
    }
}
