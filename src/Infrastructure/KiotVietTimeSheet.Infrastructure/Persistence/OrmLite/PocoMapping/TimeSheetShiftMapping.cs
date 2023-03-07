using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class TimeSheetShiftMapping
    {
        protected TimeSheetShiftMapping(){}
        public static void Mapping()
        {
            var model = typeof(TimeSheetShift);

            model.AddAttributes(new AliasAttribute("TimeSheetShift"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());

            model.GetProperty("ShiftIdsToList")
                .AddAttributes(new IgnoreAttribute());
            model.GetProperty("RepeatDaysOfWeekInList")
                .AddAttributes(new IgnoreAttribute());

            // For dto
            var readModel = typeof(TimeSheetShiftDto);
            readModel.AddAttributes(new AliasAttribute("TimeSheetDto"));

        }
    }
}
