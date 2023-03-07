using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.Allowance
{
    public class AllowanceRule : BaseRule<AllowanceRuleValue, AllowanceRuleParam>
    {
        private readonly List<ValidationFailure> _errors;
        private MainSalaryRule _mainSalaryRule;
        public AllowanceRule(AllowanceRuleValue value, AllowanceRuleParam param)
            : base(value, param)
        {
            _errors = new List<ValidationFailure>();
        }

        public override List<ValidationFailure> Errors => _errors;

        public override decimal Process()
        {
            decimal totalMoney = 0;
            Param?.Allowances?.ForEach(a =>
            {
                if (a.SelectedItem == false) return;
                //Xử lý data cũ cho KCCTL-664, KCCTL-1147: Nếu data cũ uncheck phụ cấp tính theo ngày làm việc thì sẽ reset lại thành phụ cấp cố định
                if (a.Type == AllowanceTypes.BaseOnWorkingDay && a.IsChecked != true)
                {
                    a.Type = AllowanceTypes.FixedAllowance;
                    a.IsChecked = true;
                }

                // Ưu tiên ratio hơn
                if (a.ValueRatio != null)
                {
                    totalMoney += (_mainSalaryRule?.Process() ?? 0) * (decimal)(a.ValueRatio ?? 0) / 100;
                }
                else if (a.Value != null && (a.IsChecked != false || a.Type == AllowanceTypes.FixedAllowance))
                {
                    switch (a.Type)
                    {
                        case AllowanceTypes.BaseOnWorkingDay:
                            totalMoney += (decimal)a.Value * a.NumberWorkingDay;
                            break;
                        case AllowanceTypes.BaseOnDayStandard:
                            //Tính tổng phụ cấp theo ngày công chuẩn
                            //Nếu ngày làm việc thực tế nhiều hơn số ngày công chuẩn thì tổng phụ cấp = mức áp dụng (a.Value)
                            var actualTotal = a.NumberWorkingDay * ((decimal)a.Value / a.StandardWorkingDayNumber);

                            totalMoney += Math.Min(actualTotal, (decimal)a.Value);
                            break;
                        case AllowanceTypes.FixedAllowance:
                        default:
                            totalMoney += (decimal)a.Value;
                            break;
                    }
                }
                
            });

            return Math.Round(totalMoney, 0);

        }

        public override void Factory(EngineResource resource)
        {
            if (resource.Rules?.Any() != true) throw new ArgumentOutOfRangeException(nameof(resource));
            _mainSalaryRule = resource.Rules.FirstOrDefault(r => r.GetType() == typeof(MainSalaryRule)) as MainSalaryRule;

            var param = new AllowanceRuleParam { Allowances = new List<AllowanceParam>() };
            if (Value.AllowanceRuleValueDetails?.Any() != true)
            {
                Param = param;
                return;
            }

            var numberWorkingDay = 0;
            //Kiểm tra xem có lựa chọn cho mỗi ngày công áp dụng
            var checkExistSelectBaseOnWorkingDay =
                Value.AllowanceRuleValueDetails.FirstOrDefault(x => x.Type == AllowanceTypes.BaseOnWorkingDay || x.Type == AllowanceTypes.BaseOnDayStandard);
            if (resource.UnPaidClockings?.Any() == true && checkExistSelectBaseOnWorkingDay != null)
                numberWorkingDay = GenNumberWorkingDay(resource);

            Value.AllowanceRuleValueDetails.ForEach(a =>
            {
                //Xử lý data cũ cho KCCTL-664, KCCTL-1147: Nếu data cũ uncheck phụ cấp tính theo ngày làm việc thì sẽ reset lại thành phụ cấp cố định
                if (a.Type == AllowanceTypes.BaseOnWorkingDay && a.IsChecked != true)
                {
                    a.Type = AllowanceTypes.FixedAllowance;
                    a.IsChecked = true;
                }

                if (param.Allowances.Any(x => x.AllowanceId == a.AllowanceId) || a.IsChecked == false) return;

                
                var allowanceParam = new AllowanceParam { AllowanceId = a.AllowanceId, Type = a.Type, IsChecked = a.IsChecked };
                // Ưu tiên ratio hơn
                allowanceParam.ValueRatio = a.ValueRatio;
                
                if (a.ValueRatio == null)
                {
                    allowanceParam.NumberWorkingDay = numberWorkingDay;
                    allowanceParam.Value = a.Value ?? 0;
                    allowanceParam.StandardWorkingDayNumber = resource.StandardWorkingDayNumber;
                }

                param.Allowances.Add(allowanceParam);
            });
            // Nếu phiếu lương đang ở tình trạng tạm tính sẽ không tính các phụ cấp đã xóa
            if (resource.PaySlipStatus == (byte)PayslipStatuses.TemporarySalary || resource.PaySlipStatus == (byte)PayslipStatuses.Draft)
                param.Allowances = GenAllowances(resource, param.Allowances);
            Param = param;
        }

        public override void Init()
        {
            Param?.Allowances?.ForEach(a =>
            {
                a.CalculatedValue = a.Value;
                a.CalculatedValueRatio = a.ValueRatio;
            });
        }

        public override void UpdateParam(string ruleParam)
        {
            if (string.IsNullOrEmpty(ruleParam)) return;
            var existingAllowanceParam =
                JsonConvert.DeserializeObject(ruleParam, typeof(AllowanceRuleParam)) as AllowanceRuleParam;
            if (Param?.Allowances == null) return;

            Param.Allowances = Param.Allowances.Where(a => Value.AllowanceRuleValueDetails.Select(x => x.AllowanceId).ToList().Contains(a.AllowanceId)).ToList();

            foreach (var allowanceParam in Param.Allowances)
            {
                var existingAllowanceParamDetail =
                    existingAllowanceParam?.Allowances?.FirstOrDefault(a => a.AllowanceId == allowanceParam.AllowanceId);
                if (existingAllowanceParamDetail == null) continue;

                //Kiểm tra dữ liệu phụ cấp có do ng dùng thay đổi hay k, nếu có sẽ keep giá trị của người dùng, còn k update lại = dữ liệu tính toán.
                if (Math.Abs((existingAllowanceParamDetail.CalculatedValueRatio ?? 0) - (existingAllowanceParamDetail.ValueRatio ?? 0)) > Constant.Tolerance && existingAllowanceParamDetail.Value == null)
                {
                    allowanceParam.ValueRatio = existingAllowanceParamDetail.ValueRatio;
                    allowanceParam.Value = null;
                }

                if (existingAllowanceParamDetail.CalculatedValue != existingAllowanceParamDetail.Value && existingAllowanceParamDetail.ValueRatio == null)
                {
                    allowanceParam.Value = existingAllowanceParamDetail.Value;
                    allowanceParam.ValueRatio = null;
                }

                allowanceParam.SelectedItem = existingAllowanceParamDetail.SelectedItem;
            }
        }

        public override bool IsValid()
        {
            var validateValueResult = new AllowanceRuleValueValidator().Validate(Value);
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
            return Value != null && Value.IsEqual(rule.GetRuleValue() as AllowanceRuleValue);
        }

        #region Private methods

        private static List<AllowanceParam> GenAllowances(EngineResource resource, List<AllowanceParam> allowanceParams)
        {
            var allowances = new List<AllowanceParam>();
            if (resource.Allowances == null || !resource.Allowances.Any()) return allowances;
            var activeAllowanceIds = resource.Allowances.Where(x => !x.IsDeleted).Select(x => x.Id).ToList();
            allowances = allowanceParams.Where(x => activeAllowanceIds.Contains(x.AllowanceId)).ToList();
            return allowances;
        }

        private static int GenNumberWorkingDay(EngineResource resource)
        {
            //Tính các ngày chấm công
            var unPaidClockingDay = resource.UnPaidClockings.Where(x => x.CheckOutDate != null)
                .GroupBy(x => x.StartTime.Date)
                .Select(groupCheckOutDate => groupCheckOutDate.Key).ToList();

            //Số ngày chấm công = Số ngày chấm công - Số ngày đã chốt lương
            var numberWorkingDay = resource.PaidClockings != null && resource.PaidClockings.Any()
                ? unPaidClockingDay.Except(resource.PaidClockings.Where(x => x.CheckOutDate != null)
                    .GroupBy(x => x.StartTime.Date).Select(groupCheckOutDate => groupCheckOutDate.Key).ToList()).Count() : unPaidClockingDay.Count;

            return numberWorkingDay;
        }
        #endregion
    }
}
