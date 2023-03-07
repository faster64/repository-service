using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class ClockingMapping
    {
        protected ClockingMapping(){}
        public static void Mapping()
        {
            var model = typeof(Clocking);

            model.AddAttributes(new AliasAttribute("Clocking"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());
            model.GetProperty("ClockingHistories")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("WorkById")
                .AddAttributes(new ForeignKeyAttribute(typeof(Employee)));
            model.GetProperty("WorkBy")
                .AddAttributes(new ReferenceAttribute());
            model.GetProperty("EmployeeId")
             .AddAttributes(new ForeignKeyAttribute(typeof(Employee)));
            model.GetProperty("Employee")
               .AddAttributes(new ReferenceAttribute());
            model.GetProperty("ClockingPenalizes")
                .AddAttributes(new ReferenceAttribute());

            // For dto
            var readModel = typeof(ClockingDto);
            readModel.AddAttributes(new AliasAttribute("Clocking"));
            readModel.GetProperty("EmployeeId")
                .AddAttributes(new ForeignKeyAttribute(typeof(Employee)));
            readModel.GetProperty("Employee")
                .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("ClockingHistories")
                 .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("WorkById")
                .AddAttributes(new ForeignKeyAttribute(typeof(Employee)));
            readModel.GetProperty("WorkBy")
                .AddAttributes(new ReferenceAttribute());
            readModel.GetProperty("ClockingPenalizesDto")
                .AddAttributes(new ReferenceAttribute());
        }
    }
}
