using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Extensions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary;
using KiotVietTimeSheet.Utilities;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Factories
{
    public static class RuleFactory
    {
        private static readonly List<Type> Types = Assembly.GetAssembly(typeof(MainSalaryRule))
            .GetTypes()
            .Where(t => t.HasInterface(typeof(IRule)))
            .ToList();

        public static IRule GetRule(string ruleType, string ruleValue, string ruleParam = "")
        {
            var type = GetRuleType(ruleType);
            var valueType = GetRuleValueType(type);
            var paramType = GetRuleParamType(type);
            var rule = Activator.CreateInstance(
                type,
                JsonConvert.DeserializeObject(ruleValue, valueType),
                !string.IsNullOrEmpty(ruleParam) ? JsonConvert.DeserializeObject(ruleParam, paramType) : null
            ) as IRule;
            return rule;
        }

        public static IRule GetRuleByRuleValueType(IRuleValue ruleValue)
        {
            if (ruleValue == null) return null;
            var ex = new Exception("Rule undefined");
            var instanceRuleException = new Exception("Can't instance rule");
            var type = Types.FirstOrDefault(t => t.BaseType != null && t.BaseType.GetGenericArguments().Any(g => g == ruleValue.GetType()));
            if (type == null) throw ex;
            
            if (!(Activator.CreateInstance(type, ruleValue, null) is IRule rule)) throw instanceRuleException;
            return rule;
        }

        public static Type GetRuleTypeByRuleParam(IRuleParam ruleParam)
        {
            var ex = new Exception("Rule undefined");
            if (ruleParam == null) return null;
            var type = Types.FirstOrDefault(t => t.BaseType != null && t.BaseType.GetGenericArguments().Any(g => g == ruleParam.GetType()));
            if (type == null) throw ex;
            return type;
        }

        public static Type GetRuleTypeFromGenericTypePropertyName(string genericPropertyName)
        {
            var type = Types.FirstOrDefault(t => t.BaseType != null && t.BaseType.GetGenericArguments().Any(g => g.Name == genericPropertyName));
            return type;
        }

        private static Type GetRuleType(string ruleType)
        {  
            var ex = new Exception("Rule undefined");
            var type = Types.FirstOrDefault(t => t.Name == ruleType);
            if (type == null || type.BaseType == null) throw ex;
            return type;
        }

        private static Type GetRuleValueType(Type ruleType)
        {
            var ex = new Exception("Rule undefined");
            if (ruleType.BaseType == null) throw ex;

            var valueType = ruleType.BaseType.GetGenericArguments().FirstOrDefault(g => g.IsRuleValue());
            if (valueType == null) throw ex;
            return valueType;

        }

        private static Type GetRuleParamType(Type ruleType)
        {
            var ex = new Exception("RuleParam undefined");
            if (ruleType.BaseType == null) throw ex;

            var valueType = ruleType.BaseType.GetGenericArguments().FirstOrDefault(g => g.IsRuleParam());
            if (valueType == null) throw ex;
            return valueType;
        }

        public static List<IRule> OrderRules(List<IRule> rules)
        {
            var rulesOrdered = new List<IRule>
            {
                rules.FirstOrDefault(r => r.GetType() == typeof(MainSalaryRule)),
                rules.FirstOrDefault(r => r.GetType() == typeof(AllowanceRule)),
                rules.FirstOrDefault(r => r.GetType() == typeof(CommissionSalaryRuleV2)),
                rules.FirstOrDefault(r => r.GetType() == typeof(OvertimeSalaryRule)),
                rules.FirstOrDefault(r => r.GetType() == typeof(DeductionRule))
            };
            rulesOrdered = rulesOrdered.Where(r => r != null).ToList();
            return rulesOrdered;
        }
    }
}
