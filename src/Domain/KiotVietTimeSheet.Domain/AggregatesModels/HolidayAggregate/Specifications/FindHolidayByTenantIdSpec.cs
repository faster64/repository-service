using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Specifications
{
    public class FindHolidayByTenantIdSpec : ExpressionSpecification<Holiday>
    {
        public FindHolidayByTenantIdSpec(int tenantId)
            : base(c => c.TenantId == tenantId)
        {
        }
    }
}
