using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Extensions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class PayRateToDtoProfile : Profile
    {
        public PayRateToDtoProfile()
        {
            CreateMap<PayRate, PayRateDto>()
                .AfterMap((src, dest) =>
                {
                    if (src.PayRateDetails == null || !src.PayRateDetails.Any()) return;

                    dest.GetType()
                        .GetProperties()
                        .Where(t => t.PropertyType.IsRuleValue())
                        .ToList()
                        .ForEach(p =>
                        {
                            var ruleType = RuleFactory.GetRuleTypeFromGenericTypePropertyName(p.PropertyType.Name);
                            var detail = src.PayRateDetails.FirstOrDefault(d => d.RuleType == ruleType.Name);
                            if (detail != null)
                            {
                                p.SetValue(dest, JsonConvert.DeserializeObject(detail.RuleValue, p.PropertyType));
                            }
                        });
                });
        }
    }
}
