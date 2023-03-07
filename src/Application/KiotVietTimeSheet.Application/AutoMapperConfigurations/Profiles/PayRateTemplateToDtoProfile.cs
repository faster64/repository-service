using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Extensions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class PayRateTemplateToDtoProfile : Profile
    {
        public PayRateTemplateToDtoProfile()
        {
            CreateMap<PayRateTemplate, PayRateFormDto>()
                .AfterMap((src, dest) =>
                {
                    if (src.PayRateTemplateDetails == null || !src.PayRateTemplateDetails.Any()) return;
                    dest.GetType()
                        .GetProperties()
                        .Where(t => t.PropertyType.IsRuleValue())
                        .ToList()
                        .ForEach(p =>
                        {
                            var ruleType = RuleFactory.GetRuleTypeFromGenericTypePropertyName(p.PropertyType.Name);
                            var detail = src.PayRateTemplateDetails.FirstOrDefault(d => d.RuleType == ruleType.Name);
                            if (detail != null)
                            {
                                p.SetValue(dest, JsonConvert.DeserializeObject(detail.RuleValue, p.PropertyType));
                            }
                        });
                });
        }
    }
}
