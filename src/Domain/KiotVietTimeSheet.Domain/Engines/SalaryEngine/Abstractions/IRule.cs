using System.Collections.Generic;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions
{
    public interface IRule
    {
        List<ValidationFailure> Errors { get; }

        bool IsValid();

        RuleEntity ToEntity();

        decimal Process();

        void Factory(EngineResource resource);

        IRuleValue GetRuleValue();

        void Init();

        void UpdateParam(string ruleParam);

        bool IsEqual(IRule rule);
    }
}
