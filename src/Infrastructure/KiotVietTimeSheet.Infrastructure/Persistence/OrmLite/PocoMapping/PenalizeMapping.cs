using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class PenalizeMapping
    {
        protected PenalizeMapping() { }
        public static void Mapping()
        {
            var model = typeof(Penalize);

            model.AddAttributes(new AliasAttribute("Penalize"));

            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());

            model.GetProperty("ClockingPenalizes")
                .AddAttributes(new IgnoreAttribute());
        }
    }
}
