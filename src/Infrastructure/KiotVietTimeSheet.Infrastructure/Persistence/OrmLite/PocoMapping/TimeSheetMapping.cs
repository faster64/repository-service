using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class TimeSheetMapping
    {
        protected TimeSheetMapping(){}
        public static void Mapping()
        {
            var model = typeof(TimeSheet);

            model.AddAttributes(new AliasAttribute("TimeSheet"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("Employee")
                  .AddAttributes(new ReferenceAttribute());
            model.GetProperty("TimeSheetShifts")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("TemporaryId")
                .AddAttributes(new IgnoreAttribute());

            // For dto
            var readModel = typeof(TimeSheetDto);
            readModel.AddAttributes(new AliasAttribute("TimeSheet"));

            readModel.GetProperty("Employee")
                  .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("TimeSheetShifts")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
