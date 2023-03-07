using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.MainSalary;
using System.Collections.Generic;
using System.Linq;

namespace KiotVietTimeSheet.Application.Utilities
{
    public static class HaftShiftUtils
    {
        #region Tính ngày nghỉ có phép
        public static decimal CalculateAuthorisedAbsence(PayslipDto payslip, SettingObjectDto settingObjectDto)
        {
            double authorisedAbsences = 0;
            var grListPaySlipClocking =
                    payslip.PayslipClockings?
                        .GroupBy(x => x.StartTime.Date)
                        .Select(x => new
                        {
                            dateKey = x.Key,
                            payslipClokings = x.Select(g => g)
                        }).ToList();

            // Tính nửa phép cho trường hợp lương theo ngày công chuẩn và tính nữa công active
            if (settingObjectDto.HalfShiftIsActive && payslip.MainSalaryRuleValue?.Type == MainSalaryTypes.Day && grListPaySlipClocking != null && grListPaySlipClocking.Any())
            {
                foreach (var grItem in grListPaySlipClocking)
                {
                    authorisedAbsences += CalculateTimeOffWithHaftShift(grItem.payslipClokings, settingObjectDto);
                }
                return (decimal)authorisedAbsences;
            }

            // Tính nghỉ phép cho các trường hợp khác.
            authorisedAbsences = grListPaySlipClocking.Select(x =>
                    x.payslipClokings.Any(y => y.AbsenceType != (byte)AbsenceTypes.AuthorisedAbsence)
                        ? 0
                        : 1)
                .Sum();

            return (decimal)authorisedAbsences;
        }

        private static double CalculateTimeOffWithHaftShift(IEnumerable<PayslipClockingDto> clockingDtos, SettingObjectDto settingObjectDto)
        {
            var authorisedAbsenceTime = DayTimeOff(clockingDtos.ToList());
            var halfShiftSettingTime = settingObjectDto.HalfShiftMaxHour * 60 + settingObjectDto.HalfShiftMaxMinute;
            if (authorisedAbsenceTime > 0)
            {
                return authorisedAbsenceTime > halfShiftSettingTime ? 1 : 0.5;
            }

            return 0;
        }

        private static double DayTimeOff(List<PayslipClockingDto> clockings)
        {
            // lấy thời gian của nghỉ có phép
            var authorisedAbsences = clockings.Where(c => c.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence).ToList();
            if (authorisedAbsences != null && authorisedAbsences.Any())
            {
                return clockings
                    .Where(c => c.AbsenceType == (byte)AbsenceTypes.AuthorisedAbsence)
                    .Select(c => (c.EndTime - c.StartTime).TotalMinutes).Sum();
            }
            return 0;
        }
        #endregion
    }
}
