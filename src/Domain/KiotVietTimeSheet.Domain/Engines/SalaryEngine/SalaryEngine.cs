using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine
{
    public class SalaryEngine : ISalaryEngine
    {
        #region Properties
        private readonly IList<IRule> _rules;
        private readonly decimal _mainSalary;
        private readonly decimal _allowance;
        private readonly decimal _deduction;
        private readonly decimal _commissionSalary;
        private readonly decimal _overtimeSalary;
        private readonly decimal _bonus;
        /// <summary>
        /// Vi phạm
        /// </summary>
        private readonly decimal _totalPenalizeMoney;

        #endregion
        public SalaryEngine(IList<IRule> rules, decimal bonus = 0, decimal totalPenalizeMoney = 0)
        {
            _rules = rules;
            _bonus = bonus;
            _totalPenalizeMoney = totalPenalizeMoney;
        }

        public SalaryEngine(
            decimal mainSalary = 0,
            decimal allowance = 0,
            decimal deduction = 0,
            decimal commissionSalary = 0,
            decimal overtimeSalary = 0,
            decimal bonus = 0
            )
        {
            _mainSalary = mainSalary;
            _allowance = allowance;
            _deduction = deduction;
            _commissionSalary = commissionSalary;
            _overtimeSalary = overtimeSalary;
            _bonus = bonus;
        }

        public decimal CalculateNetSalary()
        {
            if (_rules != null)
            {
                return _rules.Where(r => r.GetType() != typeof(DeductionRule)).Sum(r => r.Process()) + _bonus -
                       (_rules.FirstOrDefault(r => r.GetType() == typeof(DeductionRule))?.Process() ?? 0) - _totalPenalizeMoney;
            }

            return Math.Round(_mainSalary, 0) + Math.Round(_allowance, 0) + Math.Round(_commissionSalary, 0) + Math.Round(_overtimeSalary, 0) + Math.Round(_bonus, 0) - Math.Round(_deduction, 0) - Math.Round(_totalPenalizeMoney, 0);
        }

        public decimal CalculateGrossSalary()
        {
            if (_rules != null)
            {
                return _rules.Where(r => r.GetType() != typeof(DeductionRule)).Sum(r => r.Process()) + _bonus;
            }

            return Math.Round(_mainSalary, 0) + Math.Round(_allowance, 0) + Math.Round(_commissionSalary, 0) + Math.Round(_overtimeSalary, 0) + Math.Round(_bonus, 0);
        }
    }
}
