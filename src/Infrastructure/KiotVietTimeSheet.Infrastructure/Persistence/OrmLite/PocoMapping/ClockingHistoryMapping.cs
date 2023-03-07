using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class ClockingHistoryMapping
    {
        protected ClockingHistoryMapping(){}
        public static void Mapping()
        {
            var model = typeof(ClockingHistory);

            model.AddAttributes(new AliasAttribute("ClockingHistory"));
            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());

            // For dto
            var readModel = typeof(ClockingHistoryDto);
            readModel.AddAttributes(new AliasAttribute("ClockingHistory"));
        }
    }
}
