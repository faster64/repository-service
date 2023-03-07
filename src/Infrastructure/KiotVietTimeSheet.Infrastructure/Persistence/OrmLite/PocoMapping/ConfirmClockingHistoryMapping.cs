using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ConfirmClockingAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class ConfirmClockingHistoryMapping
    {
        protected ConfirmClockingHistoryMapping(){}
        public static void Mapping()
        {
            var model = typeof(ConfirmClockingHistory);

            model.AddAttributes(new AliasAttribute("ConfirmClockingHistory"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("ConfirmClocking")
                 .AddAttributes(new ReferenceAttribute());

            // For read model
            var readModel = typeof(ConfirmClockingHistoryDto);
            readModel.AddAttributes(new AliasAttribute("ConfirmClockingHistory"));
            readModel.GetProperty("ConfirmClocking")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
