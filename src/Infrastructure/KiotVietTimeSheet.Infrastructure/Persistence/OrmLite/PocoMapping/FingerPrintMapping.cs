using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class FingerPrintMapping
    {
        protected FingerPrintMapping(){}
        public static void Mapping()
        {
            var model = typeof(FingerPrint);

            model.AddAttributes(new AliasAttribute("FingerPrint"));
            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());
        }
    }
}
