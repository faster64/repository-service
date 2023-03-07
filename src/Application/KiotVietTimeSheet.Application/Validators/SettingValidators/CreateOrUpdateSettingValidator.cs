using System;
using FluentValidation;
using FluentValidation.Validators;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Validators.SettingValidators
{
    public class CreateOrUpdateSettingValidator : AbstractValidator<SettingsDto>
    {
        public CreateOrUpdateSettingValidator()
        {
            ValidateValue();
            ValidateStartDateOfTwiceAMonth();
        }

        public void ValidateStartDateOfTwiceAMonth()
        {
            RuleFor(s => s)
                .Must(setting => setting.FirstStartDateOfTwiceAMonth != setting.SecondStartDateOfTwiceAMonth)
                .WithMessage(Message.setting_startAndEndDateTwiceAMonthEqually);
        }

        protected void ValidateValue()
        {
            RuleFor(s => s)
                .Custom((s, context) =>
                {
                    var isValid = AddFailureMonth(context, s);
                    if (!isValid)
                    {
                        return;
                    }

                    if (!ValidateWeekValue(s.StartDayOfWeekEveryWeek))
                    {
                        context.AddFailure(string.Format(Message.setting_notValidStartDateWorkingPeriod,
                            PaySheetWorkingPeriodStatuses.EveryWeek.ToDescription()));
                        return;
                    }

                    if (!ValidateWeekValue(s.StartDayOfWeekTwiceWeekly))
                    {
                        context.AddFailure(string.Format(Message.setting_notValidStartDateWorkingPeriod,
                            PaySheetWorkingPeriodStatuses.TwiceWeekly.ToDescription()));
                        return;
                    }

                    if (s.StandardWorkingDay < 1 || s.StandardWorkingDay > 24)
                    {
                        context.AddFailure(Message.setting_notValidTimeOfStandardWorkingDay);
                    }

                    AddValidTimeSheet(s, context);

                    if (!s.IsAutoTimekeepingMultiple) return;

                    AddValidateShifṭ(s, context);

                    if (!ValidateHalfShiftMax(s))
                    {
                        context.AddFailure(Message.setting_halfShiftMaxTimeInvalid);
                    }
                });
        }

        private void AddValidTimeSheet(SettingsDto s, CustomContext context)
        {
            if (!ValidateTimeSheetValue(s.EarlyTime))
            {
                context.AddFailure(Message.setting_notValidEarlyTime);
            }

            if (!ValidateTimeSheetOtValue(s.EarlyTimeOT))
            {
                context.AddFailure(Message.setting_notValidEarlyTimeOt);
            }

            if (!ValidateTimeSheetValue(s.LateTime))
            {
                context.AddFailure(Message.setting_notValidLateTime);
            }

            if (!ValidateTimeSheetOtValue(s.LateTimeOT))
            {
                context.AddFailure(Message.setting_notValidLateTimeOt);
            }
        }

        private void AddValidateShifṭ(SettingsDto s, CustomContext context)
        {
            if (!ValidateMaxShiftValue(s.MaxShiftIsAutoTimekeepingMultiple))
            {
                context.AddFailure(Message.setting_notValidMaxShift);
            }

            if (!ValidateRangeShiftWithHoursValue(s.RangeShiftIsAutoTimekeepingMultipleHours))
            {
                context.AddFailure(Message.setting_notValidRangeShift);
            }

            if (!ValidateRangeShiftWithMinutesValue(s.RangeShiftIsAutoTimekeepingMultipleMinutes))
            {
                context.AddFailure(Message.setting_notValidRangeShift);
            }
        }

        private bool AddFailureMonth(CustomContext context, SettingsDto s)
        {
            if (!ValidateMonthValue(s.StartDateOfEveryMonth))
            {
                context.AddFailure(string.Format(Message.setting_notValidStartDateWorkingPeriod,
                    PaySheetWorkingPeriodStatuses.EveryMonth.ToDescription()));
                return false;
            }

            if (!ValidateMonthValue(s.FirstStartDateOfTwiceAMonth))
            {
                context.AddFailure(string.Format(Message.setting_notValidStartDateWorkingPeriod,
                    PaySheetWorkingPeriodStatuses.TwiceAMonth.ToDescription()));
                return false;
            }

            if (ValidateMonthValue(s.SecondStartDateOfTwiceAMonth)) return true;

            context.AddFailure(string.Format(Message.setting_notValidStartDateWorkingPeriod,
                PaySheetWorkingPeriodStatuses.TwiceAMonth.ToDescription()));
            return false;
        }

        private bool ValidateMonthValue(byte monthValue)
        {
            return monthValue > 0 && monthValue <= 31;
        }

        private bool ValidateWeekValue(byte weekValue)
        {
            return weekValue >= (byte)DayOfWeek.Sunday && weekValue <= (byte)DayOfWeek.Saturday;
        }

        private bool ValidateTimeSheetValue(int timeSheetValue)
        {
            return timeSheetValue >= 0 && timeSheetValue <= 60;
        }

        private bool ValidateTimeSheetOtValue(int timeSheetOtValue)
        {
            return timeSheetOtValue >= 0 && timeSheetOtValue <= 999;
        }

        private bool ValidateMaxShiftValue(int maxShiftValue)
        {
            return maxShiftValue <= Constant.MaximumShiftIsAutoTimekeeping &&
                   maxShiftValue >= Constant.MinimumShiftIsAutoTimekeeping;
        }
        private bool ValidateRangeShiftWithHoursValue(int rangeShiftValue)
        {
            return rangeShiftValue <= Constant.MaximumRangeShiftIsAutoTimekeepingHours;
        }
        private bool ValidateRangeShiftWithMinutesValue(int rangeShiftValue)
        {
            return rangeShiftValue <= Constant.MaximumRangeShiftIsAutoTimekeepingMinutes;
        }

        private bool ValidateHalfShiftMax(SettingsDto s)
        {
            return !(s.HalfShiftIsActive && s.HalfShiftMaxHour <= 0 && s.HalfShiftMaxMinute <= 0);
        }
    }
}
