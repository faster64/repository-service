using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class PayRateMapping
    {
        protected PayRateMapping(){}
        public static void Mapping()
        {
            var model = typeof(PayRate);

            model.AddAttributes(new AliasAttribute("PayRate"));

            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("PayRateDetails")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
