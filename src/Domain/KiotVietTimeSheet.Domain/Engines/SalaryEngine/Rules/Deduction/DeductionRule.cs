using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using Newtonsoft.Json;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Deduction
{
    public class DeductionRule : BaseRule<DeductionRuleValue, DeductionRuleParam>
    {
        private readonly List<ValidationFailure> _errors;
        private decimal _totalSalary;
        public DeductionRule(DeductionRuleValue value, DeductionRuleParam param)
            : base(value, param)
        {
            _errors = new List<ValidationFailure>();
        }

        public override List<ValidationFailure> Errors => _errors;

        public override decimal Process()
        {
            decimal totalMoney = 0;
            Param?.Deductions?.ForEach(a =>
            {
                // Ưu tiên ratio hơn
                if (a.SelectedItem == false) return;
                if (a.ValueRatio != null)
                {
                    totalMoney += _totalSalary * (decimal)(a.ValueRatio ?? 0) / 100;
                }
                else
                {
                    if (a.Value != null) totalMoney += (decimal)a.Value;
                }
            });

            return Math.Round(totalMoney, 0);
        }

        public override void Factory(EngineResource resource)
        {
            if (resource.Rules == null || !resource.Rules.Any()) throw new ArgumentOutOfRangeException(nameof(resource));
            // Giảm trừ ratio sẽ tính giá trị dựa trên tổng lương
            _totalSalary = new SalaryEngine(resource.Rules, resource.Bonus).CalculateGrossSalary();

            var param = new DeductionRuleParam { Deductions = new List<DeductionParam>() };
            if (Value.DeductionRuleValueDetails == null || !Value.DeductionRuleValueDetails.Any())
            {
                Param = param;
                return;
            }
            //Đếm số lần chi tiết đi muộn
            var numberClockingsWithLateTime = resource.UnPaidClockings != null && resource.UnPaidClockings.Any()
                ? resource.UnPaidClockings.Count(x => x.TimeIsLate > 0) - resource.NumberLateTimeHaftWorkingDay
                : 0;
            var numberClockingsWithEarlyTime = resource.UnPaidClockings != null && resource.UnPaidClockings.Any()
                ? resource.UnPaidClockings.Count(x => x.TimeIsLeaveWorkEarly > 0) - resource.NumberEarlyTimeHaftWorkingDay
                : 0;

            SetCountDeductionLateTime(param, numberClockingsWithLateTime, numberClockingsWithEarlyTime, resource);

            PaySlipTemporaySalary(resource, param);

            Param = param;
        }

        /// <summary>
        /// Đếm số lần đi muộn
        /// </summary>
        /// <param name="param"></param>
        /// <param name="numberClockingsWithLateTime"></param>
        /// <param name="numberClockingsWithEarlyTime"></param>
        /// <param name="resource"></param>
        private void SetCountDeductionLateTime(DeductionRuleParam param, int numberClockingsWithLateTime, int numberClockingsWithEarlyTime, EngineResource resource)
        {
            Value.DeductionRuleValueDetails.ForEach(deductionRuleValueDetail =>
            {
                if (param.Deductions.Any(x => x.DeductionId == deductionRuleValueDetail.DeductionId)) return;
                var deductionParam = new DeductionParam { DeductionId = deductionRuleValueDetail.DeductionId, Type = (DeductionTypes)deductionRuleValueDetail.DeductionRuleId };

                if (deductionRuleValueDetail.ValueRatio != null)
                {
                    deductionParam.ValueRatio = deductionRuleValueDetail.ValueRatio;
                }
                else
                {
                    deductionParam.Value = GetDeductionParamValue(deductionRuleValueDetail, numberClockingsWithLateTime, numberClockingsWithEarlyTime, resource);
                }
                param.Deductions.Add(deductionParam);
            });
        }

        private decimal? GetDeductionParamValue(DeductionRuleValueDetail deductionRuleValueDetail, int numberClockingsWithLateTime, int numberClockingsWithEarlyTime, EngineResource resource)
        {
            decimal? value;
            var unPaidClockings = resource.UnPaidClockings.Where(c => !resource.HalfShiftDays.Contains(c.StartTime.Date)).ToList();
            switch (deductionRuleValueDetail.DeductionRuleId)
            {
                case (int)DeductionTypes.Late:
                    if (deductionRuleValueDetail.DeductionTypeId == (int)DeductionRuleTypes.Time || deductionRuleValueDetail.DeductionTypeId == 0) //Xử lý dữ liệu cũ khi DeductionType là 0
                        value = deductionRuleValueDetail.Value * numberClockingsWithLateTime;
                    else
                    {
                        var numberBlocks = GetNumberBlock(unPaidClockings, deductionRuleValueDetail.BlockTypeMinuteValue, true);
                        value = (decimal)Math.Round((double)deductionRuleValueDetail.Value * numberBlocks, 2);
                    }
                    break;
                case (int)DeductionTypes.Early:
                    if (deductionRuleValueDetail.DeductionTypeId == (int)DeductionRuleTypes.Time)
                        value = deductionRuleValueDetail.Value * numberClockingsWithEarlyTime;
                    else
                    {
                        var numberBlocks = GetNumberBlock(unPaidClockings, deductionRuleValueDetail.BlockTypeMinuteValue, false);
                        value = (decimal)Math.Round((double)deductionRuleValueDetail.Value * numberBlocks, 2);
                    }
                    break;
                default:
                    value = deductionRuleValueDetail.Value ?? 0;
                    break;
            }

            return value;
        }

        /// <summary>
        /// Nếu phiếu lương đang ở tình trạng tạm tính sẽ không tính các giảm trừ đã xóa
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="param"></param>
        private static void PaySlipTemporaySalary(EngineResource resource, DeductionRuleParam param)
        {
            if (resource.PaySlipStatus != (byte) PayslipStatuses.TemporarySalary &&
                resource.PaySlipStatus != (byte) PayslipStatuses.Draft) return;

            if (resource.Deductions != null && resource.Deductions.Any())
            {
                var activeDeductionIds = resource.Deductions.Where(x => !x.IsDeleted).Select(x => x.Id).ToList();
                // giảm trừ khác có DeductionId = 0
                param.Deductions = param.Deductions
                    .Where(x => activeDeductionIds.Contains(x.DeductionId) || x.DeductionId == 0).ToList();
            }
            else
            {
                param.Deductions = new List<DeductionParam>();
            }
        }

        public double GetNumberBlock(List<Clocking> clockings, decimal? value, bool isLate)
        {
            double blocks = 0;
            if (!value.HasValue || clockings == null || value.Value == 0)
                return blocks;

            if (isLate)
            {
                var numberClockingLate = clockings.Where(x => x.TimeIsLate > 0).ToList();
                foreach (var clocking in numberClockingLate)
                {
                    blocks += (clocking.TimeIsLate % value.Value == value.Value / 2)
                        ? ((double)Math.Floor(clocking.TimeIsLate / value.Value) + 0.5)
                        : (double)Math.Round(clocking.TimeIsLate / value.Value);
                }
            }
            else
            {
                var numberClockingEarly = clockings.Where(x => x.TimeIsLeaveWorkEarly > 0).ToList();
                foreach (var clocking in numberClockingEarly)
                {
                    blocks += (clocking.TimeIsLeaveWorkEarly % value.Value == value.Value / 2)
                        ? ((double)Math.Floor(clocking.TimeIsLeaveWorkEarly / value.Value) + 0.5)
                        : (double)Math.Round(clocking.TimeIsLeaveWorkEarly / value.Value);
                }
            }
            return blocks;
        }
        public override void Init()
        {
            Param?.Deductions?.ForEach(d =>
            {
                d.CalculatedValue = d.Value;
                d.CalculatedValueRatio = d.ValueRatio;
            });
        }

        public override void UpdateParam(string ruleParam)
        {
            if (string.IsNullOrEmpty(ruleParam)) return;
            var existingDeductionParam =
                JsonConvert.DeserializeObject(ruleParam, typeof(DeductionRuleParam)) as DeductionRuleParam;
            if (Param?.Deductions == null) return;

            Param.Deductions = Param.Deductions.Where(d => Value.DeductionRuleValueDetails.Select(x => x.DeductionId).ToList().Contains(d.DeductionId)).ToList();

            foreach (var deductionParam in Param.Deductions)
            {
                var existingDeductionParamDetail =
                    existingDeductionParam?.Deductions?.FirstOrDefault(d => d.DeductionId == deductionParam.DeductionId);
                if (existingDeductionParamDetail == null) continue;

                //Kiểm tra dữ liệu phụ cấp có do ng dùng thay đổi hay k, nếu có sẽ keep giá trị của người dùng, còn k update lại = dữ liệu tính toán.
                if (deductionParam.Type == DeductionTypes.Late)
                {
                    deductionParam.SelectedItem = existingDeductionParamDetail.SelectedItem;
                    continue;
                }
                
                if (Math.Abs((existingDeductionParamDetail.ValueRatio ?? 0) - (existingDeductionParamDetail.CalculatedValueRatio ?? 0)) > Constant.Tolerance 
                    && existingDeductionParamDetail.Value == null)
                {
                    deductionParam.ValueRatio = existingDeductionParamDetail.ValueRatio;
                    deductionParam.Value = null;
                }

                if (existingDeductionParamDetail.CalculatedValue != existingDeductionParamDetail.Value && existingDeductionParamDetail.ValueRatio == null)
                {
                    deductionParam.Value = existingDeductionParamDetail.Value;
                    deductionParam.ValueRatio = null;
                }

                deductionParam.SelectedItem = existingDeductionParamDetail.SelectedItem;
            }

            // Trong trường không thiết lập giảm trừ và có giảm trừ khác 
            var otherDeduction = existingDeductionParam?.Deductions?.FirstOrDefault(x => x.DeductionId == 0);
            if (otherDeduction != null)
                Param.Deductions.Add(otherDeduction);
        }

        public override bool IsValid()
        {
            var validateValueResult = new DeductionRuleValueValidator().Validate(Value);
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
            return Value != null && Value.IsEqual(rule.GetRuleValue() as DeductionRuleValue);
        }
    }
}
