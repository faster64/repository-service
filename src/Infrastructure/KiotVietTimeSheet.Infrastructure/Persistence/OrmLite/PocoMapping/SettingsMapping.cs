using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class SettingsMapping
    {
        protected SettingsMapping(){}
        public static void Mapping()
        {
            var model = typeof(Settings);

            model.AddAttributes(new AliasAttribute("Settings"));
            model.GetProperty("Id")
                 .AddAttributes(new AutoIncrementAttribute());



            // For dto
            var readModel = typeof(SettingsDto);
            readModel.AddAttributes(new AliasAttribute("Settings"));

        }
    }
}
