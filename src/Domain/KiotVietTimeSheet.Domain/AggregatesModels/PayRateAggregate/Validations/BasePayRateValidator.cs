using System.Collections.Generic;
using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Resources;

namespace KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Validations
{
    public class BasePayRateValidator<T> : AbstractValidator<T> where T : PayRate
    {
        protected readonly List<IRule> _rules;

        public BasePayRateValidator() { }

        public BasePayRateValidator(List<IRule> rules)
        {
            _rules = rules;
        }

        protected void ValidateEmployee()
        {
            RuleFor(payRate => payRate.EmployeeId)
                .Must(employeeId => employeeId > 0)
                .WithMessage(Message.payRate_cannotDeterminedEmployee);
        }

        protected void ValidateSalaryPeriod()
        {
            RuleFor(payRate => payRate.SalaryPeriod)
                .Must(salaryPeriod => System.Enum.GetName(typeof(PaySheetWorkingPeriodStatuses), salaryPeriod) != null)
                .WithMessage(Message.payRate_cannotDeterminedWorkingPeriod);
        }
    }
}
