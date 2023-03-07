using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications
{
    public class FindTenantNationalHolidayByLastCreatedYearSpec : ExpressionSpecification<TenantNationalHoliday>
    {
        public FindTenantNationalHolidayByLastCreatedYearSpec(int year)
            : base(n => n.LastCreatedYear == year) { }
    }
}
