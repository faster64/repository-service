using KiotVietTimeSheet.Application.Dto;
using ServiceStack;
using ServiceStack.DataAnnotations;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;

namespace KiotVietTimeSheet.Infrastructure.Persistence.OrmLite.PocoMapping
{
    public class PayRateTemplateMapping
    {
        protected PayRateTemplateMapping(){}
        public static void Mapping()
        {
            var model = typeof(PayRateTemplate);

            model.GetProperty("PayRateTemplateDetails")
                 .AddAttributes(new ReferenceAttribute());

            var dto = typeof(PayRateTemplateDto);
            dto.AddAttributes(new AliasAttribute("PayRateTemplate"));
        }
    }
}
