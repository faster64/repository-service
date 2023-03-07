using FluentValidation;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Validators;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Resources;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Application.Validators.TimeSheetValidators
{
    public class OverlapTimeValidator : BaseTimeSheetValidator<List<TimeSheet>>
    {
        #region Properties
        #endregion

        public OverlapTimeValidator(
            List<ClockingDto> clockingsOverlapTime,
            List<Clocking> timeSheetClockings
        )
        {
            ValidateClockingsOverlap(clockingsOverlapTime, timeSheetClockings);
        }

        #region Protected methods
        // Thông báo lỗi trùng thời gian
        protected void ValidateClockingsOverlap(List<ClockingDto> clockingsOverlapTime, List<Clocking> timeSheetClockings)
        {
            RuleFor(x => x)
               .Custom((timeSheets, context) =>
               {
                   if (timeSheets == null || !timeSheets.Any()) return;
                   var listOverlap = GenListOverlap(clockingsOverlapTime);
                   var isAllTimeSheetNotValid = IsAllTimeSheetNotValid(timeSheetClockings, timeSheets);
                   var messageDuplicate = Message.timeSheet_duplicated;
                   var messageHasClocking = Message.timeSheet_haveNoClockings;

                   if (timeSheets.Count > 1)
                   {
                        messageDuplicate = Message.timeSheet_allDuplicated;
                        messageHasClocking = Message.timeSheet_allHaveNoClockings;
                   }

                   var message = messageHasClocking;

                   if (listOverlap.Any()) message = string.Format(messageDuplicate, string.Join(", ", listOverlap));

                   if (isAllTimeSheetNotValid) context.AddFailure(message);
               });
        }

        private static List<string> GenListOverlap(IReadOnlyCollection<ClockingDto> clockingsOverlapTime)
        {
            var listOverlap = new List<string>();
            if (!clockingsOverlapTime.Any()) return listOverlap;

            var isOverlapBranchOffDay = clockingsOverlapTime.Any(x => x.OverlapType == (byte)OverlapType.BranchOffDay);
            var isOverlapHoliday = clockingsOverlapTime.Any(x => x.OverlapType == (byte)OverlapType.Holiday);
            var isOverlapTimeSheet = clockingsOverlapTime.Any(x => x.OverlapType == (byte)OverlapType.TimeSheet);

            if (isOverlapBranchOffDay) listOverlap.Add(Message.timeSheet_duplicatedBranchDayOff);

            if (isOverlapHoliday) listOverlap.Add(Label.holiday.ToLower());

            if (isOverlapTimeSheet) listOverlap.Add(Message.timeSheet_duplicatedTimeSheet);

            return listOverlap;
        }

        private static bool IsAllTimeSheetNotValid(IReadOnlyCollection<Clocking> timeSheetClockings, List<TimeSheet> timeSheets)
        {
            if (timeSheetClockings == null) return false;

            var isAllTimeSheetNotValid = timeSheets.All(timeSheet =>
            {
                var timesheetId = timeSheet.TemporaryId;
                if (timeSheet.Id > 0) timesheetId = timeSheet.Id;

                var activeClockings = timeSheetClockings
                    .Where(x => x.TimeSheetId == timesheetId)
                    .Where(x => x.ClockingStatus != (byte)ClockingStatuses.Void)
                    .Where(x => !x.IsDeleted);

                return !activeClockings.Any();
            });
            return isAllTimeSheetNotValid;
        }
        #endregion
    }
}

