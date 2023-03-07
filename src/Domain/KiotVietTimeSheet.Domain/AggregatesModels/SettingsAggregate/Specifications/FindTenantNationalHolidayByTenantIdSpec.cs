using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications
{
    public class FindTenantNationalHolidayByTenantIdSpec : ExpressionSpecification<TenantNationalHoliday>
    {
        public FindTenantNationalHolidayByTenantIdSpec(int tenantId) : base(n => n.TenantId == tenantId) { }
    }
}
