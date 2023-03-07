using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Extensions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Helpers
{
    public static class SalaryRuleHelpers
    {
        public static List<IRule> GetRulesFromObjectByRuleValue(object obj)
        {
            var rules = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj) != null && p.PropertyType.IsRuleValue())
                .Select(t => RuleFactory.GetRuleByRuleValueType(t.GetValue(obj) as IRuleValue))
                .ToList();

            return rules;
        }

        public static List<IRule> GetRulesFromObjectByRuleParamAndPayRateDetail(object obj,
            List<PayRateDetail> payRateDetails)
        {
            var rules = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj) != null && p.PropertyType.IsRuleParam())
                .Select(t =>
                {
                    var ruleParam = t.GetValue(obj) as IRuleParam;
                    var type = RuleFactory.GetRuleTypeByRuleParam(ruleParam);
                    var payRateDetail = payRateDetails?.FirstOrDefault(p => p.RuleType == type.Name);
                    return payRateDetail == null ? null : RuleFactory.GetRule(type.Name, payRateDetail.RuleValue, JsonConvert.SerializeObject(ruleParam));
                })
                .ToList();

            return rules;
        }
    }
}
