using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommissionSalary
{
    public class CommissionSalaryRule : BaseRule<CommissionSalaryRuleValue, CommissionSalaryRuleParam>
    {
        private readonly List<ValidationFailure> _errors;

        public CommissionSalaryRule(CommissionSalaryRuleValue value, CommissionSalaryRuleParam param)
            : base(value, param)
        {
            _errors = new List<ValidationFailure>();
        }

        public override List<ValidationFailure> Errors => _errors;

        public override decimal Process()
        {
            if (Param == null) throw new ArgumentOutOfRangeException(); //NOSONAR
            decimal total;
            if (Param.CommissionSalary != Param.CalculatedCommissionSalary)
                return Param.CommissionSalary;

            var minimumRevenue = Value.UseMinCommission ? Value.MinCommission ?? 0 : 0;
            var revenueToCalculateSalary = Param.TotalRevenue < minimumRevenue ? 0 : Param.TotalRevenue - minimumRevenue;
            var suitableCommissionSalaryRuleValueDetails = Value.CommissionSalaryRuleValueDetails?
                .Where(c => c.Commission <= revenueToCalculateSalary).OrderByDescending(c => c.Commission).ToList();

            if (suitableCommissionSalaryRuleValueDetails == null || !suitableCommissionSalaryRuleValueDetails.Any())
            {
                return 0;
            }
            var maximumSuitableCommissionSalaryRuleValueDetail = suitableCommissionSalaryRuleValueDetails.First();
            if (maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio == null)
            {
                total = maximumSuitableCommissionSalaryRuleValueDetail.Value ?? 0;
            }
            else
            {
                total = revenueToCalculateSalary *
                        (decimal)(maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio ?? 0) / 100;
            }

            return Math.Round(total, 0);
        }

        public override void Factory(EngineResource resource)
        {
            var param = new CommissionSalaryRuleParam();
            if (Value.CommissionSalaryRuleValueDetails == null || !Value.CommissionSalaryRuleValueDetails.Any())
            {
                Param = param;
                return;
            }

            var minimumRevenue = Value.UseMinCommission ? Value.MinCommission ?? 0 : 0;
            var revenueToCalculateSalary = resource.TotalRevenue < minimumRevenue ? 0 : resource.TotalRevenue - minimumRevenue;

            // Lấy giá trị gần nhất và nhỏ hơn doanh thu thuần
            var suitableCommissionSalaryRuleValueDetails = 
                Value.CommissionSalaryRuleValueDetails.Where(c => c.Commission <= revenueToCalculateSalary)
                                                      .OrderByDescending(c => c.Commission)
                                                      .ToList();

            if (suitableCommissionSalaryRuleValueDetails.Any())
            {
                var maximumSuitableCommissionSalaryRuleValueDetail = suitableCommissionSalaryRuleValueDetails.First();
                if (maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio == null)
                {
                    param.Value = maximumSuitableCommissionSalaryRuleValueDetail.Value ?? 0;
                }
                else
                {
                    param.ValueRatio = maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio ?? 0;
                }
            }
            param.TotalRevenue = resource.TotalRevenue;
            Param = param;
            Param.CommissionSalary = Process();
        }

        public override void Init()
        {
            Param.CalculatedCommissionSalary = Param.CommissionSalary;
            Param.CalculatedValue = Param.Value;
            Param.CalculatedValueRatio = Param.ValueRatio;
        }

        public override void UpdateParam(string ruleParam)
        {
            if (string.IsNullOrEmpty(ruleParam)) return;
            var existingCommissionParam =
                JsonConvert.DeserializeObject(ruleParam, typeof(CommissionSalaryRuleParam)) as CommissionSalaryRuleParam;
            if (Param == null || existingCommissionParam == null) return;

            var isChangeTotalRevenue = Param.TotalRevenue != existingCommissionParam.TotalRevenue;

            // Trong TH thay đổi doanh thu bán hàng
            // => Tính toán lại lương kinh doanh nếu setup theo mức %
            if (isChangeTotalRevenue && existingCommissionParam.ValueRatio != null && existingCommissionParam.Value == null)
            {
                CaculateCommissionSalary(existingCommissionParam);
            }
            else if (existingCommissionParam.CommissionSalary != existingCommissionParam.CalculatedCommissionSalary)
                        Param.CommissionSalary = existingCommissionParam.CommissionSalary;

            if (existingCommissionParam.Value != existingCommissionParam.CalculatedValue && existingCommissionParam.ValueRatio == null)
            {
                Param.Value = existingCommissionParam.Value;
                Param.ValueRatio = null;

                // Trong TH thay đổi mức áp dụng phiếu lương từ % sang VND (hoặc ngược lại) nhưng không thay đổi lương kinh doanh
                // => lương kinh doanh = giữ lại lương người dùng thay đổi
                if (existingCommissionParam.CommissionSalary == existingCommissionParam.CalculatedCommissionSalary && existingCommissionParam.Value != null)
                    Param.CommissionSalary = (decimal)existingCommissionParam.Value;
            }

            if (CheckCommissionParam(existingCommissionParam)) return;

            Param.ValueRatio = existingCommissionParam.ValueRatio;
            Param.Value = null;

            // Trong TH thay đổi mức áp dụng phiếu lương từ % sang VND (hoặc ngược lại) nhưng không thay đổi lương kinh doanh
            // => lương kinh doanh = giữ lại lương người dùng thay đổi
            CaculateCommissionSalaryWhenChangePercentToVnd(isChangeTotalRevenue, existingCommissionParam);
        }

        public override bool IsValid()
        {
            var validateValueResult = new CommissionSalaryRuleValueValidator().Validate(Value);
            if (!validateValueResult.IsValid)
            {
                _errors.AddRange(validateValueResult.Errors);
            }
            return !_errors.Any();
        }

        public override bool IsEqual(IRule rule)
        {
            if (rule == null) return false;
            if (Value == null && rule.GetRuleValue() == null) return true;
            return Value != null && Value.IsEqual(rule.GetRuleValue() as CommissionSalaryRuleValue);
        }

        #region Private methods

        private void CaculateCommissionSalary(CommissionSalaryRuleParam existingCommissionParam)
        {
            var minimumRevenue = (decimal)0;
            if (Value.UseMinCommission) minimumRevenue = Value.MinCommission ?? 0;

            var revenueToCalculateSalary = (decimal)0;
            if (Param.TotalRevenue >= minimumRevenue)
                revenueToCalculateSalary = Param.TotalRevenue - minimumRevenue;

            // Lấy giá trị gần nhất và nhỏ hơn doanh thu thuần
            var suitableCommissionSalaryRuleValueDetails = Value.CommissionSalaryRuleValueDetails?
                .Where(c => c.Commission <= revenueToCalculateSalary).OrderByDescending(c => c.Commission).ToList();

            if (suitableCommissionSalaryRuleValueDetails == null ||
                !suitableCommissionSalaryRuleValueDetails.Any()) return;

            var maximumSuitableCommissionSalaryRuleValueDetail = suitableCommissionSalaryRuleValueDetails.First();

            var hasValueRatio = maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio != null &&
                                Math.Abs((existingCommissionParam.ValueRatio ?? 0) -
                                         (maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio ?? 0)) >
                                Constant.Tolerance &&
                                Math.Abs((existingCommissionParam.CalculatedValueRatio ?? 0) -
                                         (existingCommissionParam.ValueRatio ?? 0)) < Constant.Tolerance;
            if (hasValueRatio)
            {
                Param.CommissionSalary = Param.TotalRevenue *
                                         (decimal)maximumSuitableCommissionSalaryRuleValueDetail.ValueRatio /
                                         100;
                if (Value.UseMinCommission)
                    Param.CommissionSalary = revenueToCalculateSalary *
                                             (decimal)maximumSuitableCommissionSalaryRuleValueDetail
                                                 .ValueRatio / 100;
            }
            else
            {
                if (existingCommissionParam.ValueRatio == null) return;
                Param.CommissionSalary =
                    Param.TotalRevenue * (decimal) existingCommissionParam.ValueRatio / 100;
                if (Value.UseMinCommission)
                    Param.CommissionSalary =
                        revenueToCalculateSalary * (decimal) existingCommissionParam.ValueRatio / 100;
            }
        }

        private static bool CheckCommissionParam(CommissionSalaryRuleParam existingCommissionParam)
        {
            var calculatedValueRatio = existingCommissionParam.CalculatedValueRatio ?? 0;
            var valueRatio = existingCommissionParam.CalculatedValueRatio ?? 0;
            return existingCommissionParam.Value != null || (Math.Abs(calculatedValueRatio - valueRatio) <= Constant.Tolerance);
        }

        private void CaculateCommissionSalaryWhenChangePercentToVnd(bool isChangeTotalRevenue, CommissionSalaryRuleParam existingCommissionParam)
        {
            if (isChangeTotalRevenue || existingCommissionParam.CommissionSalary !=
                existingCommissionParam.CalculatedCommissionSalary) return;
            if (existingCommissionParam.ValueRatio != null)
                Param.CommissionSalary = existingCommissionParam.TotalRevenue *
                                         (decimal)existingCommissionParam.ValueRatio / 100;
        }
        #endregion
    }
}
