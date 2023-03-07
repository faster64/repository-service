using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class GpsInfoMapping
    {
        protected GpsInfoMapping(){}
        public static void Mapping()
        {
            var model = typeof(GpsInfo);

            model.AddAttributes(new AliasAttribute("GpsInfo"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("ConfirmClockings")
                 .AddAttributes(new ReferenceAttribute());

            // For read model
            var readModel = typeof(GpsInfoDto);
            readModel.AddAttributes(new AliasAttribute("GpsInfo"));
            readModel.GetProperty("ConfirmClockings")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
