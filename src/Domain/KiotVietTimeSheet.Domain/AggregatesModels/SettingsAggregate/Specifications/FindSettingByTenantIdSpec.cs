using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications
{
    public class FindSettingByTenantIdSpec : ExpressionSpecification<Settings>
    {
        public FindSettingByTenantIdSpec(int tenantId)
            : base((s) => s.TenantId == tenantId)
        { }
    }


}
