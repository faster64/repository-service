using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class AllowanceMapping
    {
        protected AllowanceMapping(){}
        public static void Mapping()
        {
            var model = typeof(Allowance);

            model.AddAttributes(new AliasAttribute("Allowance"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
        }
    }
}
