using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class BranchSettingMapping
    {
        protected BranchSettingMapping(){}
        public static void Mapping()
        {
            var model = typeof(BranchSetting);

            model.AddAttributes(new AliasAttribute("BranchSetting"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("BranchId")
                 .AddAttributes(new IndexAttribute(true));
            model.GetProperty("WorkingDaysInArray")
                .AddAttributes(new IgnoreAttribute());

        }
    }
}
