using System.Linq;
using AutoMapper;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Extensions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Application.AutoMapperConfigurations.Profiles
{
    public class MappingPayslipToPayslipDto : Profile
    {
        public MappingPayslipToPayslipDto()
        {
            CreateMap<Payslip, PayslipDto>()
                .AfterMap((src, dest) =>
                {
                    if (src.PayslipDetails == null || !src.PayslipDetails.Any()) return;
                    dest.GetType()
                        .GetProperties()
                        .Where(t => t.PropertyType.IsRuleParam() || t.PropertyType.IsRuleValue())
                        .ToList()
                        .ForEach(p =>
                        {
                            var ruleType = RuleFactory.GetRuleTypeFromGenericTypePropertyName(p.PropertyType.Name);
                            var detail = src.PayslipDetails.FirstOrDefault(d => d.RuleType == ruleType.Name);
                            if (detail == null) return;

                            p.SetValue(dest,
                                p.PropertyType.IsRuleParam()
                                    ? JsonConvert.DeserializeObject(detail.RuleParam, p.PropertyType)
                                    : JsonConvert.DeserializeObject(detail.RuleValue, p.PropertyType)
                            );
                        });
                });
        }
    }
}
