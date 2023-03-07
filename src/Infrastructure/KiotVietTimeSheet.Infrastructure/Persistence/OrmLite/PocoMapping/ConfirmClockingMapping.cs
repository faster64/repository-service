using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class ConfirmClockingMapping
    {
        protected ConfirmClockingMapping(){}
        public static void Mapping()
        {
            var model = typeof(ConfirmClocking);

            model.AddAttributes(new AliasAttribute("ConfirmClocking"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("ConfirmClockingHistories")
                 .AddAttributes(new ReferenceAttribute());
            model.GetProperty("Employee")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("GpsInfo")
                .AddAttributes(new ReferenceAttribute());

            // For read model
            var readModel = typeof(ConfirmClockingDto);
            readModel.AddAttributes(new AliasAttribute("ConfirmClocking"));
            readModel.GetProperty("ConfirmClockingHistories")
                .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("Employee")
                .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("GpsInfo")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
