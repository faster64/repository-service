
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications
{
    public class FindSettingByIdSpec : ExpressionSpecification<Settings>
    {
        public FindSettingByIdSpec(long id)
            : base((s) => s.Id == id)
        { }
    }
}
