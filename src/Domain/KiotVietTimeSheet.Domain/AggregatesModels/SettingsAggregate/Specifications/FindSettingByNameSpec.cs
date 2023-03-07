
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Specifications
{
    public class FindSettingByNameSpec : ExpressionSpecification<Settings>
    {
        public FindSettingByNameSpec(string name)
            : base((s) => s.Name == name)
        { }
    }
}
