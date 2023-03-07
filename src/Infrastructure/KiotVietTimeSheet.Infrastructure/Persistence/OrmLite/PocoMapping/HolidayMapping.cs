using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using ServiceStack.DataAnnotations;
using ServiceStack;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class HolidayMapping
    {
        public static void Mapping()
        {
            var model = typeof(Holiday);

            model.AddAttributes(new AliasAttribute("Holiday"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("Days")
                 .AddAttributes(new CustomSelectAttribute("CAST(DATEDIFF(DAY, \"From\", \"To\") AS INT) + 1"));
        }
    }
}
