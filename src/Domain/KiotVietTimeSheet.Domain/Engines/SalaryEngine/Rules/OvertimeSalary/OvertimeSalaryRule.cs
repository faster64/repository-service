using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.OvertimeSalary
{
    public class OvertimeSalaryRule : BaseRule<OvertimeSalaryRuleValue, OvertimeSalaryRuleParam>
    {
        #region Properties
        private List<OvertimeSalaryByShiftParam> _ruleFromClockings = new List<OvertimeSalaryByShiftParam>();
        private readonly List<ValidationFailure> _errors;
        private MainSalaryRule _mainSalaryRule;
        private int _standardWorkingDayNumber;
        private int _timeOfStandardWorkingDay;
        #endregion

        public OvertimeSalaryRule(OvertimeSalaryRuleValue value, OvertimeSalaryRuleParam param)
            : base(value, param)
        {
            _errors = new List<ValidationFailure>();
        }

        public override List<ValidationFailure> Errors => _errors;

        public override decimal Process()
        {
            if (Param?.OvertimeSalaryByShifts == null) throw new ArgumentOutOfRangeException(); //NOSONAR
            decimal total = 0;
            if (_mainSalaryRule == null) return total;

            var mainSalaryRuleValue = _mainSalaryRule.GetMainSalaryValue();
            switch (mainSalaryRuleValue.Type)
            {
                case MainSalaryTypes.Hour:
                    total = ProcessByShifts(mainSalaryRuleValue);
                    break;
                case MainSalaryTypes.Shift:
                    total = ProcessByShifts(mainSalaryRuleValue);
                    break;
                case MainSalaryTypes.Day:
                    total = ProcessByDay(mainSalaryRuleValue);
                    break;
                case MainSalaryTypes.Fixed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();//NOSONAR
            }

            return Math.Round(total, 0);
        }

        public override void Factory(EngineResource resource)
        {
            if (resource.Rules == null || !resource.Rules.Any()) throw new ArgumentOutOfRangeException(nameof(resource));
            var mainSalaryRule = resource.Rules.FirstOrDefault(r => r.GetType() == typeof(MainSalaryRule)) as MainSalaryRule;
            // Lấy các dữ liệu cần thiết từ EngineResource
            _mainSalaryRule = mainSalaryRule;
            _timeOfStandardWorkingDay = resource.TimeOfStandardWorkingDay;
            _standardWorkingDayNumber = resource.StandardWorkingDayNumber;

            var mainSalaryRuleValue = _mainSalaryRule?.GetMainSalaryValue();
            if (mainSalaryRuleValue == null)
            {
                Param = new OvertimeSalaryRuleParam { OvertimeSalaryByShifts = new List<OvertimeSalaryByShiftParam>() };
                return;
            }

            switch (mainSalaryRuleValue.Type)
            {
                case MainSalaryTypes.Day:
                    Param = new OvertimeSalaryRuleParam
                    {
                        OvertimeSalaryByShifts = new List<OvertimeSalaryByShiftParam>
                    {
                        new OvertimeSalaryByShiftParam
                        {
                            ShiftId = 0,
                            OvertimeSalaryByShiftParamDays = new List<OvertimeSalaryByShiftParamDays>
                            {
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(resource.UnPaidClockings
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue) == SalaryDays.Saturday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Saturday
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(resource.UnPaidClockings
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue) == SalaryDays.Sunday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Sunday
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(resource.UnPaidClockings
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue) == SalaryDays.DayOff)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.DayOff
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(resource.UnPaidClockings
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue) == SalaryDays.Default)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Default
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(resource.UnPaidClockings
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue) == SalaryDays.Holiday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Holiday
                                }
                            }
                        }
                    }
                    };
                    _ruleFromClockings.AddRange(Param.OvertimeSalaryByShifts);
                    break;
                default:
                    var result = new List<OvertimeSalaryByShiftParam>();
                    var ruleFromClockings = resource.UnPaidClockings
                        .GroupBy(c => c.ShiftId)
                        .Select(groupShift =>
                        {
                        // chia trung bình để lấy thời gian làm viêc của ca dựa trên các chi tiết làm việc trên ca đó (trường hợp thời gian của ca bị thay đổi trong khoảng tgian tinh lương)
                        var shiftHours = groupShift.Average(c => (c.EndTime).Subtract(c.StartTime).TotalHours);

                            return new OvertimeSalaryByShiftParam
                            {
                                ShiftId = groupShift.Key,
                                ShiftHours = shiftHours,
                                OvertimeSalaryByShiftParamDays = new List<OvertimeSalaryByShiftParamDays>()
                                {
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue, groupShift.Key, shiftHours) == SalaryDays.Saturday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Saturday
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue, groupShift.Key, shiftHours) == SalaryDays.Sunday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Sunday
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue, groupShift.Key, shiftHours) == SalaryDays.DayOff)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.DayOff
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue, groupShift.Key, shiftHours) == SalaryDays.Default)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Default
                                },
                                new OvertimeSalaryByShiftParamDays()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, mainSalaryRuleValue, groupShift.Key, shiftHours) == SalaryDays.Holiday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Holiday
                                }
                                }
                            };
                        })
                        .ToList();

                    // lưu dữ liệu làm thêm từ chi tiết làm việc
                    _ruleFromClockings = ruleFromClockings;
                    result.AddRange(ruleFromClockings);

                    // lấy dữ liệu làm thêm không có chi tiết làm việc (người dùng tự thêm trên phiếu lương)
                    var shiftIds = ruleFromClockings.Select(x => x.ShiftId).ToList();
                    var shifts = resource.Shifts.Where(x => !shiftIds.Contains(x.Id)).ToList();
                    var listOvertimeSalaryByShift = CalculateShiftHours(shifts);
                    if(listOvertimeSalaryByShift != null)
                        result.AddRange(listOvertimeSalaryByShift);
                    Param = new OvertimeSalaryRuleParam
                    {
                        OvertimeSalaryByShifts = result
                    };
                    break;
            }
        }

        private List<OvertimeSalaryByShiftParam> CalculateShiftHours(List<Shift> shifts)
        {
            var listOvertimeSalaryByShift = new List<OvertimeSalaryByShiftParam>();
            if (!shifts.Any()) return new List<OvertimeSalaryByShiftParam>();
            foreach (var shift in shifts)
            {
                var shiftHours =
                    shift.To > shift.From ? (double)(shift.To - shift.From) / 60 : 24 - (double)(shift.From - shift.To) / 60;

                listOvertimeSalaryByShift.Add(new OvertimeSalaryByShiftParam
                {
                    ShiftId = shift.Id,
                    ShiftHours = shiftHours > 0 ? Math.Round(shiftHours, 2) : 0
                });
            }
            return listOvertimeSalaryByShift;
        }

        public override void Init()
        {
            Param?.OvertimeSalaryByShifts?.ForEach(m =>
            {
                m.OvertimeSalaryByShiftParamDays?.ForEach(x =>
                {
                    if (_ruleFromClockings.Select(c => c.ShiftId).Contains(m.ShiftId))
                    {
                        x.CalculatedValue = x.Value;
                    }
                });

            });
        }

        public override void UpdateParam(string ruleParam)
        {
            //
        }

        public override bool IsValid()
        {
            var validateValueResult = new OvertimeSalaryRuleValueValidator().Validate(Value);
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
            return Value != null && Value.IsEqual(rule.GetRuleValue() as OvertimeSalaryRuleValue);
        }

        #region Private methods
        private decimal ProcessByShifts(MainSalaryRuleValue mainSalaryRuleValue)
        {
            decimal total = 0;

            Param.OvertimeSalaryByShifts.ForEach(detail =>
            {
                var salaryOneHour = GetMainSalary(mainSalaryRuleValue, detail.ShiftId, detail.ShiftHours);
                total += Calculate(Value, salaryOneHour, detail.OvertimeSalaryByShiftParamDays);
            });
            return total;
        }

        private decimal ProcessByDay(MainSalaryRuleValue mainSalaryRuleValue)
        {
            if (_standardWorkingDayNumber <= 0 || _timeOfStandardWorkingDay <= 0) return 0;
            var salaryOneDay = GetMainSalary(mainSalaryRuleValue);
            return Calculate(Value, salaryOneDay, Param.OvertimeSalaryByShifts.FirstOrDefault()?.OvertimeSalaryByShiftParamDays);
        }

        private decimal Calculate(OvertimeSalaryRuleValue conditionValue, decimal salaryOneHour, List<OvertimeSalaryByShiftParamDays> overtimeSalaryByShiftParamDays)
        {
            decimal total = 0;
            if (conditionValue?.OvertimeSalaryDays == null || !conditionValue.OvertimeSalaryDays.Any()) return total;

            var dayDefault = conditionValue.OvertimeSalaryDays.FirstOrDefault(x => x.Type == SalaryDays.Default);
            foreach (var day in conditionValue.OvertimeSalaryDays)
            {
                var numberDay = overtimeSalaryByShiftParamDays?.FirstOrDefault(x => x.Type == day.Type);
                if (numberDay == null) continue;

                if (day.IsApply)
                {
                    total += CalculateTotalMoneyOverTime(day.MoneyTypes, numberDay.Value, day.Value, salaryOneHour);
                    continue;
                }

                //nếu ngày đó không áp dụng sẽ tính về ngày thường
                if (dayDefault == null || !dayDefault.IsApply)
                {
                    continue;
                }

                total += CalculateTotalMoneyOverTime(dayDefault.MoneyTypes, numberDay.Value, dayDefault.Value,
                    salaryOneHour);
            }

            return total;
        }

        private decimal CalculateTotalMoneyOverTime(MoneyTypes moneyType, decimal numberDay, decimal dayValue, decimal salaryOneHour)
        {
            return moneyType == MoneyTypes.Money ? numberDay * dayValue : (salaryOneHour * dayValue / 100) * numberDay;
        }

        private SalaryDays CheckClockingDayTypes(EngineResource resource, Clocking clocking, OvertimeSalaryRuleValue conditionValue, MainSalaryRuleValue mainSalaryRuleValue, long shitId = 0, double shiftHours = 0)
        {
            var lstDayClocking = new List<SalaryDays>();
            if (!resource.BranchSetting.WorkingDaysInArray.Contains((byte)clocking.StartTime.DayOfWeek))
            {
                lstDayClocking.Add(SalaryDays.DayOff);
            }

            if ((byte)clocking.StartTime.DayOfWeek == (byte)SalaryDays.Sunday)
            {
                lstDayClocking.Add(SalaryDays.Sunday);
            }

            if ((byte)clocking.StartTime.DayOfWeek == (byte)SalaryDays.Saturday)
            {
                lstDayClocking.Add(SalaryDays.Saturday);
            }

            if (resource.Holidays.Any(holiday =>
                clocking.StartTime.Date >= holiday.From.Date && clocking.StartTime.Date < holiday.To))
            {
                lstDayClocking.Add(SalaryDays.Holiday);
            }

            //Trường hợp 1 ca làm việc thuộc cùng 1 lúc nhiều ngày hệ thống sẽ lấy ca thuộc ngày mức lương cao nhất
            if (lstDayClocking.Count > 1)
            {
                return GetListClockingWithMaxSalary(lstDayClocking, conditionValue, mainSalaryRuleValue, shitId, shiftHours);
            }

            return lstDayClocking.Count == 1 ? lstDayClocking.First() : SalaryDays.Default;
        }

        private SalaryDays GetListClockingWithMaxSalary(List<SalaryDays> lstDayClocking, OvertimeSalaryRuleValue conditionValue, MainSalaryRuleValue mainSalaryRuleValue, long shitId = 0, double shiftHours = 0)
        {
            var mainSalaryDefault = GetMainSalary(mainSalaryRuleValue, shitId, shiftHours);

            var getMaxValueSalary =
                    (from dayClocking in lstDayClocking
                     from overtimeSalaryDay in conditionValue.OvertimeSalaryDays.Where(x => x.IsApply)
                     where dayClocking == overtimeSalaryDay.Type
                     select new OvertimeSalaryDays
                     {
                        Type = overtimeSalaryDay.Type,
                        Value = overtimeSalaryDay.MoneyTypes == MoneyTypes.Percent
                            ? (mainSalaryDefault * overtimeSalaryDay.Value / 100)
                            : overtimeSalaryDay.Value
                     }).ToList();

            OvertimeSalaryDays result = getMaxValueSalary.FirstOrDefault();

            if (result == null) return lstDayClocking.FirstOrDefault();

            foreach (var days in getMaxValueSalary.Skip(1))
            {
                result = days != null && days.Value > result.Value ? days : result;
            }

            return getMaxValueSalary.Any() ? result.Type : lstDayClocking.First();
        }

        private double SumParam(Clocking clocking) => (clocking.OverTimeBeforeShiftWork + clocking.OverTimeAfterShiftWork) / (double)60;

        private decimal GetMainSalary(MainSalaryRuleValue mainSalaryRuleValue, long shitId = 0, double shiftHours = 0)
        {
            decimal mainSalary = 0;
            var mainSalaryValueDetail =
                mainSalaryRuleValue.MainSalaryValueDetails.FirstOrDefault(m => m.ShiftId == shitId) ??
                mainSalaryRuleValue.MainSalaryValueDetails.FirstOrDefault(m => m.ShiftId == 0);
            if (mainSalaryValueDetail == null) return 0;

            switch (mainSalaryRuleValue.Type)
            {
                case MainSalaryTypes.Day:
                    if(_standardWorkingDayNumber > 0 && _timeOfStandardWorkingDay > 0)
                        mainSalary = mainSalaryValueDetail.Default / _standardWorkingDayNumber /
                                     _timeOfStandardWorkingDay;
                    break;
                case MainSalaryTypes.Shift:
                    mainSalary = ((decimal)shiftHours <= 0 ? 0 : mainSalaryValueDetail.Default / (decimal)shiftHours);
                    break;
                default:
                    mainSalary = mainSalaryValueDetail.Default;
                    break;
            }

            return mainSalary;
        }
        #endregion
    }
}
