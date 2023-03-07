using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class DeductionMapping
    {
        protected DeductionMapping(){}
        public static void Mapping()
        {
            var model = typeof(Deduction);

            model.AddAttributes(new AliasAttribute("Deduction"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
        }
    }
}
