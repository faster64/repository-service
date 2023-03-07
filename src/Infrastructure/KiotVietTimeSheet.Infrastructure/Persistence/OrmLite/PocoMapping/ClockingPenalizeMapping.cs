using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PenalizeAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class ClockingPenalizeMapping
    {
        protected ClockingPenalizeMapping() { }
        public static void Mapping()
        {
            var model = typeof(ClockingPenalize);

            model.AddAttributes(new AliasAttribute("ClockingPenalize"));
            model.GetProperty("Id")
                .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("PenalizeId")
                .AddAttributes(new ForeignKeyAttribute(typeof(Penalize)));
            model.GetProperty("Penalize")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("ClockingId")
                .AddAttributes(new ForeignKeyAttribute(typeof(Clocking)));
            model.GetProperty("Clocking")
                .AddAttributes(new ReferenceAttribute());

            // For dto
            var readModel = typeof(ClockingPenalizeDto);
            readModel.AddAttributes(new AliasAttribute("ClockingPenalize"));
            readModel.GetProperty("PenalizeId")
                .AddAttributes(new ForeignKeyAttribute(typeof(PenalizeDto)));
            readModel.GetProperty("PenalizeDto")
                .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("ClockingId")
                .AddAttributes(new ForeignKeyAttribute(typeof(ClockingDto)));
            readModel.GetProperty("ClockingDto")
                .AddAttributes(new IgnoreAttribute());
            readModel.GetProperty("ShiftId")
                .AddAttributes(new IgnoreAttribute());
            readModel.GetProperty("ClockingPenalizeCreated")
                .AddAttributes(new IgnoreAttribute());
        }
    }
}
