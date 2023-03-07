using System.Collections.Generic;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions
{
    public abstract class BaseRule<TValue, TParam> : IRule
        where TValue : IRuleValue
        where TParam : IRuleParam
    {
        protected TValue Value { get; set; }
        protected TParam Param { get; set; }

        protected BaseRule(TValue value, TParam param)
        {
            Value = value;
            Param = param;
        }

        public abstract List<ValidationFailure> Errors { get; }

        public abstract decimal Process();

        public abstract void Factory(EngineResource resource);

        public abstract bool IsValid();

        public abstract void Init();

        public abstract void UpdateParam(string ruleParam);

        public abstract bool IsEqual(IRule rule);

        public RuleEntity ToEntity()
        {
            return new RuleEntity
            {
                Type = GetType().Name,
                Value = JsonConvert.SerializeObject(Value),
                Param = JsonConvert.SerializeObject(Param)
            };
        }
        public IRuleValue GetRuleValue()
        {
            return Value;
        }
    }
}
