using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Enum;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary
{
    public class MainSalaryRule : BaseRule<MainSalaryRuleValue, MainSalaryRuleParam>
    {
        #region Properties
        private int _standardWorkingDayNumber;
        private readonly List<MainSalaryByShiftParam> _ruleFromClocking = new List<MainSalaryByShiftParam>();
        private int _holidays = 0;
        #endregion
        public MainSalaryRule(MainSalaryRuleValue value, MainSalaryRuleParam param)
            : base(value, param)
        { }

        private readonly List<ValidationFailure> _errors = new List<ValidationFailure>();
        public override List<ValidationFailure> Errors => _errors;

        public override void Factory(EngineResource resource)
        {
            _standardWorkingDayNumber = resource.StandardWorkingDayNumber;
            switch (Value.Type)
            {
                case MainSalaryTypes.Hour:
                case MainSalaryTypes.Shift:
                    Param = FactoryByShifts(resource);
                    break;
                case MainSalaryTypes.Day:
                    Param = FactoryByDay(resource);
                    break;
                case MainSalaryTypes.Fixed:
                    Param = FactoryByFixed(resource);
                    break;
            }
        }

        public override void Init()
        {
            Param?.MainSalaryShifts?.ForEach(m =>
            {
                m.MainSalaryByShiftParamDetails?.ForEach(x =>
                {
                    if (_ruleFromClocking.Select(c => c.ShiftId).Contains(m.ShiftId))
                    {
                        x.CalculatedValue = x.Value;
                    }
                });
                m.CalculatedDefault = m.Default - _holidays;
                m.CalculatedSalary = m.Salary;
            });
        }

        public override void UpdateParam(string ruleParam)
        {
            if (string.IsNullOrEmpty(ruleParam)) return;
            if (Param?.MainSalaryShifts == null) return;

            Param.MainSalaryShifts = Param.MainSalaryShifts.Where(mp => Value.MainSalaryValueDetails.Select(mv => mv.ShiftId).ToList().Contains(mp.ShiftId)).ToList();

            foreach (var mainSalaryParam in Param.MainSalaryShifts)
            {
                var mainSalaryValue = Value?.MainSalaryValueDetails?.FirstOrDefault(m => m.ShiftId == mainSalaryParam.ShiftId);

                if (mainSalaryValue == null)
                {
                    Param.MainSalaryShifts.Remove(mainSalaryParam);
                }
            }
        }

        public override bool IsValid()
        {
            var validateValueResult = new MainSalaryRuleValueValidator().Validate(Value);
            if (!validateValueResult.IsValid)
            {
                _errors.AddRange(validateValueResult.Errors);
            }
            return !_errors.Any();
        }

        public override decimal Process()
        {
            decimal total = 0;
            if (Param?.MainSalaryShifts == null) return total;

            switch (Value.Type)
            {
                case MainSalaryTypes.Hour:
                    total = ProcessByShifts();
                    break;
                case MainSalaryTypes.Shift:
                    total = ProcessByShifts();
                    break;
                case MainSalaryTypes.Day:
                    total = ProcessByDay();
                    break;
                case MainSalaryTypes.Fixed:
                    total = ProcessByFixed();
                    break;
            }

            return Math.Round(total, 0);
        }

        public MainSalaryRuleValue GetMainSalaryValue() => Value;

        public override bool IsEqual(IRule rule)
        {
            if (rule == null) return false;
            if (Value == null && rule.GetRuleValue() == null) return true;
            return Value != null && Value.IsEqual(rule.GetRuleValue() as MainSalaryRuleValue);
        }

        #region Private methods
        private decimal ProcessByShifts()
        {
            decimal total = 0;
            if (Value.MainSalaryValueDetails == null || Value.MainSalaryValueDetails.Count == 0) return total;
            Value.MainSalaryValueDetails.ForEach(detail =>
            {
                var paramForShift = Param.MainSalaryShifts.FirstOrDefault(f => f.ShiftId == detail.ShiftId);
                if (paramForShift != null)
                {
                    total += Calculate(detail, paramForShift, detail.Default);
                }
                // Lương chính mặc định
                else if (detail.ShiftId == 0)
                {
                    var mainSalaryShiftDefault = Param.MainSalaryShifts.FirstOrDefault(m => m.ShiftId == 0);
                    total += Calculate(detail, mainSalaryShiftDefault, detail.Default);
                }
            });
            return total;
        }

        private decimal ProcessByFixed()
        {
            if (Param?.MainSalaryShifts != null)
            {
                return Param.MainSalaryShifts.FirstOrDefault()?.Salary ?? 0;
            }
            return Value.MainSalaryValueDetails.FirstOrDefault()?.Default ?? 0;
        }

        private decimal ProcessByDay()
        {
            decimal total = 0;
            if (_standardWorkingDayNumber <= 0) return 0;
            if (Value.MainSalaryValueDetails == null || Value.MainSalaryValueDetails.Count == 0 ||
                Value.MainSalaryValueDetails.Count > 1) return 0;
            Value.MainSalaryValueDetails.ForEach(detail =>
            {
                var mainSalaryShiftDefault = Param.MainSalaryShifts.FirstOrDefault(m => m.ShiftId == 0);
                total += Calculate(detail, mainSalaryShiftDefault, detail.Default / _standardWorkingDayNumber);
            });

            return total;
        }

        private MainSalaryRuleParam FactoryByShifts(EngineResource resource)
        {
            var result = new MainSalaryRuleParam();
            var ruleFromClocking = resource.UnPaidClockings
                .GroupBy(c => c.ShiftId)
                .Select(groupShift =>
                {
                    return new MainSalaryByShiftParam
                    {
                        // Các clockings có ca làm việc không đc cài đặt mức lương sẽ set ShiftId chung = 0
                        ShiftId = Value.MainSalaryValueDetails.Any(d => d.ShiftId > 0 && d.ShiftId == groupShift.Key)
                            ? groupShift.Key
                            : 0,
                        Default = (decimal)Math.Round(groupShift
                            .Where(c => CheckClockingDayTypes(resource, c, Value, groupShift.Key) == SalaryDays.Default)
                            .Sum(c => SumParam(c)), 2),
                        MainSalaryByShiftParamDetails = new List<MainSalaryByShiftParamDetail>()
                        {
                                new MainSalaryByShiftParamDetail()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, groupShift.Key) == SalaryDays.Saturday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Saturday,
                                },
                                new MainSalaryByShiftParamDetail()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, groupShift.Key) == SalaryDays.Sunday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Sunday,
                                },
                                new MainSalaryByShiftParamDetail()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, groupShift.Key) == SalaryDays.DayOff)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.DayOff,
                                },
                                new MainSalaryByShiftParamDetail()
                                {
                                    Value = (decimal)Math.Round(groupShift
                                        .Where(c => CheckClockingDayTypes(resource, c, Value, groupShift.Key) == SalaryDays.Holiday)
                                        .Sum(c => SumParam(c)),2),
                                    Type = SalaryDays.Holiday,
                                }
                        }
                    };
                })
                .ToList();
            _ruleFromClocking.AddRange(ruleFromClocking);
            // lấy giữ liệu lương không có chi tiết làm việc (người dùng tự thêm trên phiếu lương)
            var shiftIds = ruleFromClocking.Select(x => x.ShiftId).ToList();
            var ruleFromPayRates = Value.MainSalaryValueDetails.Where(x => !shiftIds.Contains(x.ShiftId)).ToList();

            foreach (var rule in ruleFromPayRates)
            {
                ruleFromClocking.Add(new MainSalaryByShiftParam
                {
                    ShiftId = rule.ShiftId
                });
            }
            result.MainSalaryShifts = ruleFromClocking;

            // Gộp các param mức lương mặc định, hoặc khởi tạo 1 object mặc định
            var mainSalaryShiftsOfDefaultMainSalary = result.MainSalaryShifts.Where(m => m.ShiftId == 0).ToList();
            var mainSalaryShiftOfDefaultDetail =
                mainSalaryShiftsOfDefaultMainSalary.Where(x => x.MainSalaryByShiftParamDetails != null).ToList();
            result.MainSalaryShifts = result.MainSalaryShifts.Where(m => m.ShiftId > 0).ToList();
            result.MainSalaryShifts.Add(new MainSalaryByShiftParam
            {
                ShiftId = 0,
                Default = mainSalaryShiftsOfDefaultMainSalary.Select(x => x.Default).Sum(),
                MainSalaryByShiftParamDetails = new List<MainSalaryByShiftParamDetail>()
                    {
                       new MainSalaryByShiftParamDetail()
                       {
                           Value = Math.Round(mainSalaryShiftOfDefaultDetail.SelectMany(x=>x.MainSalaryByShiftParamDetails.Where(y=>y.Type == SalaryDays.Saturday)).Sum(m => m.Value), 2),
                           Type = SalaryDays.Saturday,
                       },
                       new MainSalaryByShiftParamDetail()
                       {
                           Value = Math.Round(mainSalaryShiftOfDefaultDetail.SelectMany(x=>x.MainSalaryByShiftParamDetails.Where(y=>y.Type == SalaryDays.Sunday)).Sum(m => m.Value), 2),
                           Type = SalaryDays.Sunday,
                       },
                       new MainSalaryByShiftParamDetail()
                       {
                           Value = Math.Round(mainSalaryShiftOfDefaultDetail.SelectMany(x=>x.MainSalaryByShiftParamDetails.Where(y=>y.Type == SalaryDays.DayOff)).Sum(m => m.Value), 2),
                           Type = SalaryDays.DayOff,
                       },
                       new MainSalaryByShiftParamDetail()
                       {
                           Value = Math.Round(mainSalaryShiftOfDefaultDetail.SelectMany(x=>x.MainSalaryByShiftParamDetails.Where(y=>y.Type == SalaryDays.Holiday)).Sum(m => m.Value), 2),
                           Type = SalaryDays.Holiday,
                       }
                    },
                Type = Value.Type
            });
            return result;
        }
        private MainSalaryRuleParam FactoryByFixed(EngineResource resource)
        {
            var result = new MainSalaryRuleParam
            {
                MainSalaryShifts = new List<MainSalaryByShiftParam>
                {
                    new MainSalaryByShiftParam
                    {
                        ShiftId = 0,
                        Type = Value.Type,
                        Default = resource.UnPaidClockings.GroupBy(c => c.StartTime.Date).Count()
                    }
                }
            };
            result.MainSalaryShifts.ForEach(mainSalary =>
            {
                mainSalary.Salary = Value.MainSalaryValueDetails.FirstOrDefault()?.Default ?? 0;
            });
            _ruleFromClocking.AddRange(result.MainSalaryShifts);

            return result;
        }

        private double CalculateWorkingDays(EngineResource resource, SalaryDays salaryDays)
        {
            double standardWorkingDayNumber = 0;
            var UnPaidClockingsGrouped = (resource.UnPaidClockings
                .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == salaryDays)
                .GroupBy(c => c.StartTime.Date)
                .Select(g => new
                {
                    Day = g.Key,
                    Clockings = g.ToList()
                }));
            List<StandardWorkingDayDetail> standardWorkingDayDetails = new List<StandardWorkingDayDetail>();
            foreach (var upcg in UnPaidClockingsGrouped)
            {
                StandardWorkingDayDetail swdd = new StandardWorkingDayDetail
                {
                    Day = upcg.Day,
                    Late = upcg.Clockings.Count(x => x.TimeIsLate > 0),
                    Early = upcg.Clockings.Count(c => c.TimeIsLeaveWorkEarly > 0),
                    StandardWorkingTime = DayTimeWorking(upcg.Clockings)
                };
                standardWorkingDayDetails.Add(swdd);
            }

            foreach (var item in standardWorkingDayDetails)
            {
                if (item.StandardWorkingTime > (resource.SettingsToObject.HalfShiftMaxHour * 60 + resource.SettingsToObject.HalfShiftMaxMinute))
                {
                    standardWorkingDayNumber += 1;
                }
                else
                {
                    resource.NumberEarlyTimeHaftWorkingDay += item.Early;
                    resource.NumberLateTimeHaftWorkingDay += item.Late;
                    standardWorkingDayNumber += 0.5;

                    // Lấy những ngày nữa công ra để không tính đi trễ về sớm
                    resource.HalfShiftDays.Add(item.Day);
                }
            }

            return standardWorkingDayNumber;
        }

        private double DayTimeWorking(List<Clocking> clockings)
        {
            double dayTimeWorking = 0;
            var authorisedAbsence = clockings.Where(c => c.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence).ToList();
            if (authorisedAbsence != null && authorisedAbsence.Any())
                dayTimeWorking += authorisedAbsence.Select(c => (c.EndTime - c.StartTime).TotalMinutes).Sum();

            dayTimeWorking += clockings
                .Where(c => c.AbsenceType == null)
                .Where(c => c.CheckOutDate.HasValue)
                .GroupBy(c => c.StartTime.Date)
                .Select(g => g.Sum(
                    c => ((c.CheckInDate.HasValue ? (c.CheckOutDate.Value - c.CheckInDate.Value) : (c.CheckOutDate.Value - c.StartTime)).TotalMinutes) - (c.OverTimeAfterShiftWork + c.OverTimeBeforeShiftWork)
                    )).FirstOrDefault();
            return dayTimeWorking;
        }

        private MainSalaryRuleParam FactoryByDay(EngineResource resource)
        {
            // holidays not work
            var holidays = CalculateHolidaysForDefault(resource);
            _holidays = holidays;

            var result = new MainSalaryRuleParam
            {
                MainSalaryShifts = new List<MainSalaryByShiftParam>
                {
                    new MainSalaryByShiftParam
                    {
                        ShiftId = 0,
                        Type = Value.Type,
                        CalculatedDefault = (resource.SettingsToObject.HalfShiftIsActive
                            ? (decimal)CalculateWorkingDays(resource, SalaryDays.Default)
                            : (resource.UnPaidClockings
                                .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.Default)
                                .GroupBy(c => c.StartTime.Date).Count())),
                        Default = (resource.SettingsToObject.HalfShiftIsActive
                            ? (decimal)CalculateWorkingDays(resource, SalaryDays.Default)
                            : (resource.UnPaidClockings
                                .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.Default)
                                .GroupBy(c => c.StartTime.Date).Count())) + holidays,
                        MainSalaryByShiftParamDetails = new List<MainSalaryByShiftParamDetail>()
                        {
                            new MainSalaryByShiftParamDetail()
                            {
                                Value = (resource.SettingsToObject.HalfShiftIsActive
                                    ? (decimal)CalculateWorkingDays(resource, SalaryDays.Saturday)
                                    : (resource.UnPaidClockings
                                    .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.Saturday)
                                    .GroupBy(c => c.StartTime.Date).Count())),
                                Type = SalaryDays.Saturday,
                            },
                            new MainSalaryByShiftParamDetail()
                            {
                                Value = (resource.SettingsToObject.HalfShiftIsActive
                                    ? (decimal)CalculateWorkingDays(resource, SalaryDays.Sunday)
                                    : (resource.UnPaidClockings
                                    .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.Sunday)
                                    .GroupBy(c => c.StartTime.Date).Count())),
                                Type = SalaryDays.Sunday,
                            },
                            new MainSalaryByShiftParamDetail()
                            {
                                Value = (resource.SettingsToObject.HalfShiftIsActive
                                    ? (decimal)CalculateWorkingDays(resource, SalaryDays.DayOff)
                                    : (resource.UnPaidClockings
                                    .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.DayOff)
                                    .GroupBy(c => c.StartTime.Date).Count())),
                                Type = SalaryDays.DayOff,
                            },
                            new MainSalaryByShiftParamDetail()
                            {
                                Value = (resource.SettingsToObject.HalfShiftIsActive
                                    ? (decimal)CalculateWorkingDays(resource, SalaryDays.Holiday)
                                    : (resource.UnPaidClockings
                                    .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.Holiday)
                                    .GroupBy(c => c.StartTime.Date).Count())),
                                Type = SalaryDays.Holiday,
                            }
                        }
                    }
                }
            };
            _ruleFromClocking.AddRange(result.MainSalaryShifts);
            return result;
        }

        private int CalculateHolidaysForDefault(EngineResource resource)
        {
            // Tổng số ngày lễ trong tháng
            var holidays = 0;
            foreach (var holiday in resource.Holidays)
            {
                if (holiday.Days > 1)
                {
                    for (DateTime date = holiday.From; date <= holiday.To; date = date.AddDays(1.0))
                    {
                        if (date >= resource.StartTime && date <= resource.EndTime)
                        {
                            holidays += 1;
                        }
                    }
                }
                else
                {
                    holidays += holiday.Days;
                }
            }

            // Số ngày lễ có đi làm.
            var clockings = (resource.UnPaidClockings
                                    .Where(c => CheckClockingDayTypes(resource, c, Value, 0) == SalaryDays.Holiday)
                                    .GroupBy(c => c.StartTime.Date).Count());
            return holidays - clockings;
        }

        private static decimal Calculate(MainSalaryRuleValueDetail conditionValue, MainSalaryByShiftParam mainSalaryByShiftParams, decimal salaryPerDay)
        {
            if (mainSalaryByShiftParams?.MainSalaryByShiftParamDetails == null || conditionValue?.MainSalaryHolidays == null)
            {
                return 0;
            }

            //tính lương ngày mặc định
            var total = salaryPerDay * mainSalaryByShiftParams.Default;

            //tính lương các ngày khác
            foreach (var dayParam in mainSalaryByShiftParams.MainSalaryByShiftParamDetails)
            {
                var daySetting = conditionValue.MainSalaryHolidays?.FirstOrDefault(x => x.Type == dayParam.Type);

                if (daySetting == null || !daySetting.IsApply)
                {
                    total += salaryPerDay * dayParam.Value;
                }
                else if (daySetting.MoneyTypes == MoneyTypes.Money)
                {
                    total += daySetting.Value * dayParam.Value;
                }
                else
                {
                    total += salaryPerDay * (daySetting.Value / 100) * dayParam.Value;
                }
            }

            return total;
        }

        private SalaryDays CheckClockingDayTypes(EngineResource resource, Clocking clocking, MainSalaryRuleValue value, long shitId = 0)
        {
            var lstDayClocking = new List<SalaryDays>();

            var numberDayOfWeek = (byte)clocking.StartTime.DayOfWeek;

            if (resource.BranchSetting.WorkingDaysInArray.All(x => x != numberDayOfWeek))
            {
                lstDayClocking.Add(SalaryDays.DayOff);
            }

            switch (numberDayOfWeek)
            {
                case (byte)SalaryDays.Sunday:
                    lstDayClocking.Add(SalaryDays.Sunday);
                    break;
                case (byte)SalaryDays.Saturday:
                    lstDayClocking.Add(SalaryDays.Saturday);
                    break;
            }

            var startDate = clocking.StartTime.Date;
            if (resource.Holidays.Any(holiday => holiday.From.Date <= startDate && startDate < holiday.To))
            {
                lstDayClocking.Add(SalaryDays.Holiday);
            }

            if (!lstDayClocking.Any()) return SalaryDays.Default;

            if (lstDayClocking.Count == 1) return lstDayClocking.First();

            //Trường hợp 1 ca làm việc thuộc cùng 1 lúc nhiều ngày hệ thống sẽ lấy ca thuộc ngày mức lương cao nhất
            var getShiftSalaryValue = value.MainSalaryValueDetails.FirstOrDefault(x => x.ShiftId == shitId) ??
                                      value.MainSalaryValueDetails.FirstOrDefault(x => x.ShiftId == 0);

            if (getShiftSalaryValue == null) return lstDayClocking.First();

            var mainSalaryDefault = getShiftSalaryValue.Default;

            var getMaxValueSalary =
                (from mainSalary in getShiftSalaryValue.MainSalaryHolidays.Where(x => x.IsApply)
                 from clDay in lstDayClocking
                 where mainSalary.Type == clDay
                 select new MainSalaryHolidays
                 {
                     Type = mainSalary.Type,
                     Value = mainSalary.MoneyTypes == MoneyTypes.Percent ? (mainSalaryDefault * mainSalary.Value / 100) : mainSalary.Value
                 }).ToList();

            if (getMaxValueSalary.Any())
            {
                return getMaxValueSalary.Aggregate((agg, next) =>
                    next.Value > agg.Value ? next : agg).Type;
            }

            return lstDayClocking.First();
        }

        private double SumParam(Clocking clocking)
        {
            // Với trường hợp tính theo ca làm việc 1 clocking = 1 ca
            if (Value.Type == MainSalaryTypes.Shift) return 1;

            // Với trường hợp tính theo giờ làm việc
            if (clocking.CheckOutDate != null)
            {
                return clocking.CheckOutDate.Value.Subtract(clocking.CheckInDate ?? clocking.StartTime).TotalHours;
            }
            return clocking.EndTime.Subtract(clocking.StartTime).TotalHours;
        }
        #endregion
    }
}
