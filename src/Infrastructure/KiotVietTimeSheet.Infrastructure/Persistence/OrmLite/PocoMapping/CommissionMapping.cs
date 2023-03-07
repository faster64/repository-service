using KiotVietTimeSheet.Domain.AggregatesModels.CommissionAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class CommissionMapping
    {
        protected CommissionMapping(){}
        public static void Mapping()
        {
            var model = typeof(Commission);

            model.AddAttributes(new AliasAttribute("Commission"));
            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("CommissionDetails")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("CommissionBranches")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}