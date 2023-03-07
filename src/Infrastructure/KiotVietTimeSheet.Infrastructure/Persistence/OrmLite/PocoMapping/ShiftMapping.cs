using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class ShiftMapping
    {
        protected ShiftMapping(){}
        public static void Mapping()
        {
            var model = typeof(Shift);

            model.AddAttributes(new AliasAttribute("Shift"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("DomainEvents")
                .AddAttributes(new IgnoreAttribute());
        }
    }
}
