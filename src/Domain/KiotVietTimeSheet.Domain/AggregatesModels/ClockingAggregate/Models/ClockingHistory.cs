using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models
{
    public class ClockingHistory : BaseEntity,
        IEntityIdlong,
        ITenantId,
        IBranchId,
        IModifiedBy,
        IModifiedDate,
        ICreatedBy,
        ICreatedDate
    {
        #region PROPERTIES
        public long Id { get; set; }
        public long ClockingId { get; protected set; }
        public DateTime? CheckedInDate { get; protected set; }
        public DateTime? CheckedOutDate { get; protected set; }
        public int TimeIsLate { get; protected set; }
        public int OverTimeBeforeShiftWork { get; protected set; }
        public int TimeIsLeaveWorkEarly { get; protected set; }
        public int OverTimeAfterShiftWork { get; protected set; }
        public int TenantId { get; set; }
        public int BranchId { get; set; }
        public byte TimeKeepingType { get; protected set; }
        public byte ClockingStatus { get; protected set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public byte ClockingHistoryStatus { get; protected set; }
        public int TimeIsLateAdjustment { get; protected set; }
        public int OverTimeBeforeShiftWorkAdjustment { get; protected set; }
        public int TimeIsLeaveWorkEarlyAdjustment { get; protected set; }
        public int OverTimeAfterShiftWorkAdjustment { get; protected set; }
        public byte? AbsenceType { get; set; }
        public long? ShiftId { get; protected set; }
        public long? ShiftFrom { get; protected set; }
        public long? ShiftTo { get; protected set; }
        public long? EmployeeId { get; protected set; }
        public DateTime? CheckTime { get; set; }
        public string AutoTimekeepingUid { get; set; }
        #endregion

        #region CONSTRUCTORS

        public ClockingHistory() { }

        public ClockingHistory(
            long clockingId,
            int branchId,
            int tenantId,
            DateTime? checkedInDate,
            DateTime? checkedOutDate,
            DateTime clockingStartTime,
            DateTime clockingEndTime,
            byte timeKeepingType,
            SettingsToObject settings,
            byte clockingStatus,
            int timeIsLate,
            int overTimeBeforeShiftWork,
            int timeIsLeaveWorkEarly,
            int overTimeAfterShiftWork,
            long shiftId,
            long employeeId,
            ClockingHistory lastClockingHistory = null,
            byte? absenceType = null,
            DateTime? checkTime = null,
            string autoTimekeepingUid = null,
            ClockingHistoryType? checkInDateType = null,
            ClockingHistoryType? checkOutDateType = null
        )
        {
            ClockingId = clockingId;
            CheckedInDate = CalculateCheckIn(clockingStartTime, checkedInDate, checkInDateType);
            CheckedOutDate = CalculateCheckOut(clockingEndTime, checkedOutDate, checkOutDateType);
            var calculatedTimeIsLate = CalculateTimeIsLate(clockingStartTime, CheckedInDate, settings);
            var calculatedOverTimeBeforeShiftWork = CalculateOverTimeBeforeShiftWork(clockingStartTime, CheckedInDate, settings);
            var calculatedTimeIsLeaveWorkEarly = CalculateTimeIsLeaveWorkEarly(clockingEndTime, CheckedOutDate, settings);
            var calculatedOverTimeAfterShiftWork = CalculateOverTimeAfterShiftWork(clockingEndTime, CheckedOutDate, settings);

            TimeIsLate = timeIsLate > calculatedTimeIsLate ? calculatedTimeIsLate : timeIsLate;
            OverTimeBeforeShiftWork = overTimeBeforeShiftWork > calculatedOverTimeBeforeShiftWork
                ? calculatedOverTimeBeforeShiftWork
                : overTimeBeforeShiftWork;
            TimeIsLeaveWorkEarly = timeIsLeaveWorkEarly > calculatedTimeIsLeaveWorkEarly
                ? calculatedTimeIsLeaveWorkEarly
                : timeIsLeaveWorkEarly;
            OverTimeAfterShiftWork = overTimeAfterShiftWork > calculatedOverTimeAfterShiftWork
                ? calculatedOverTimeAfterShiftWork
                : overTimeAfterShiftWork;
            TimeKeepingType = timeKeepingType;
            ClockingStatus = clockingStatus;
            ClockingHistoryStatus = (byte)ClockingHistoryStatuses.Created;
            AbsenceType = absenceType;
            BranchId = branchId;
            TenantId = tenantId;
            ShiftId = shiftId;
            ShiftFrom = clockingStartTime.Hour * 60 + clockingStartTime.Minute;
            ShiftTo = clockingEndTime.Hour * 60 + clockingEndTime.Minute;
            EmployeeId = employeeId;
            CheckTime = checkTime ?? DateTime.Now;
            AutoTimekeepingUid = autoTimekeepingUid;
            if (lastClockingHistory != null)
            {
                TimeIsLateAdjustment = TimeIsLate - lastClockingHistory.TimeIsLate;
                OverTimeBeforeShiftWorkAdjustment = OverTimeBeforeShiftWork - lastClockingHistory.OverTimeBeforeShiftWork;
                TimeIsLeaveWorkEarlyAdjustment = TimeIsLeaveWorkEarly - lastClockingHistory.TimeIsLeaveWorkEarly;
                OverTimeAfterShiftWorkAdjustment = OverTimeAfterShiftWork - lastClockingHistory.OverTimeAfterShiftWork;
            }
            else
            {
                TimeIsLateAdjustment = 0;
                OverTimeBeforeShiftWorkAdjustment = 0;
                TimeIsLeaveWorkEarlyAdjustment = 0;
                OverTimeAfterShiftWorkAdjustment = 0;
            }
        }

        // Only copy primitive values
        public ClockingHistory(ClockingHistory clockingHistory)
        {
            Id = clockingHistory.Id;
            ClockingId = clockingHistory.ClockingId;
            CheckedInDate = clockingHistory.CheckedInDate;
            CheckedOutDate = clockingHistory.CheckedOutDate;
            TimeIsLate = clockingHistory.TimeIsLate;
            OverTimeBeforeShiftWork = clockingHistory.OverTimeBeforeShiftWork;
            TimeIsLeaveWorkEarly = clockingHistory.TimeIsLeaveWorkEarly;
            OverTimeAfterShiftWork = clockingHistory.OverTimeAfterShiftWork;
            TenantId = clockingHistory.TenantId;
            BranchId = clockingHistory.BranchId;
            TimeKeepingType = clockingHistory.TimeKeepingType;
            ClockingStatus = clockingHistory.ClockingStatus;
            ModifiedBy = clockingHistory.ModifiedBy;
            ModifiedDate = clockingHistory.ModifiedDate;
            CreatedBy = clockingHistory.CreatedBy;
            CreatedDate = clockingHistory.CreatedDate;
            ClockingHistoryStatus = clockingHistory.ClockingHistoryStatus;
            TimeIsLateAdjustment = clockingHistory.TimeIsLateAdjustment;
            OverTimeBeforeShiftWorkAdjustment = clockingHistory.OverTimeBeforeShiftWorkAdjustment;
            TimeIsLeaveWorkEarlyAdjustment = clockingHistory.TimeIsLeaveWorkEarlyAdjustment;
            OverTimeAfterShiftWorkAdjustment = clockingHistory.OverTimeAfterShiftWorkAdjustment;
            AbsenceType = clockingHistory.AbsenceType;
            CheckTime = clockingHistory.CheckTime;
            if (clockingHistory.DomainEvents != null)
            {
                foreach (var domainEvent in clockingHistory.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        #region PUBLIC METHODS

        private static int CalculateOverTimeAfterShiftWork(DateTime clockingEndTime, DateTime? checkedOutDate, SettingsToObject settings)
        {
            if (checkedOutDate == null)
            {
                return 0;
            }

            var setting = settings.IsAutoCalcLateTimeOT ? settings.LateTimeOT : 0;
            clockingEndTime = clockingEndTime.AddMinutes(setting);
            return (int)Math.Round(clockingEndTime >= checkedOutDate ? 0 : (checkedOutDate ?? clockingEndTime).Subtract(clockingEndTime).TotalMinutes);
        }

        private static int CalculateTimeIsLeaveWorkEarly(DateTime clockingEndTime, DateTime? checkedOutDate, SettingsToObject settings)
        {
            if (checkedOutDate == null)
            {
                return 0;
            }

            var setting = settings.IsAutoCalcEarlyTime ? settings.EarlyTime : 0;
            clockingEndTime = clockingEndTime.AddMinutes(-setting);
            return (int)Math.Round(clockingEndTime <= checkedOutDate ? 0 : clockingEndTime.Subtract(checkedOutDate ?? clockingEndTime).TotalMinutes);
        }

        private static int CalculateTimeIsLate(DateTime clockingStartTime, DateTime? checkedInDate, SettingsToObject settings)
        {
            if (checkedInDate == null)
            {
                return 0;
            }

            var setting = settings.IsAutoCalcLateTime ? settings.LateTime : 0;
            clockingStartTime = clockingStartTime.AddMinutes(setting);
            return (int)Math.Round(clockingStartTime >= checkedInDate ? 0 : (checkedInDate ?? clockingStartTime).Subtract(clockingStartTime).TotalMinutes);
        }

        private static int CalculateOverTimeBeforeShiftWork(DateTime clockingStartTime, DateTime? checkedInDate, SettingsToObject settings)
        {
            if (checkedInDate == null)
            {
                return 0;
            }

            var setting = settings.IsAutoCalcEarlyTimeOT ? settings.EarlyTimeOT : 0;
            clockingStartTime = clockingStartTime.AddMinutes(-setting);
            return (int)Math.Round(clockingStartTime <= checkedInDate ? 0 : clockingStartTime.Subtract(checkedInDate ?? clockingStartTime).TotalMinutes);
        }

        private static DateTime? CalculateCheckIn(DateTime clockingStartTime, DateTime? checkedInDate,
            ClockingHistoryType? checkInDateType)
        {
            if (checkedInDate == null) return null;
            switch (checkInDateType)
            {
                case ClockingHistoryType.DayBefore:
                    return clockingStartTime.Date.AddDays(-1).AddMinutes(
                        checkedInDate.Value.Hour * 60 + checkedInDate.Value.Minute);
                case ClockingHistoryType.Today:
                    return clockingStartTime.Date.AddMinutes(
                        checkedInDate.Value.Hour * 60 + checkedInDate.Value.Minute);
                case ClockingHistoryType.DayAfter:
                    return clockingStartTime.Date.AddDays(1).AddMinutes(
                        checkedInDate.Value.Hour * 60 + checkedInDate.Value.Minute);
                default:
                    //Trường hợp chấm công bằng máy chấm công
                    return checkedInDate;
            }
        }

        private static DateTime? CalculateCheckOut(DateTime clockingEndTime, DateTime? checkedOutDate,
            ClockingHistoryType? checkOutDateType)
        {
            if (checkedOutDate == null) return null;
            switch (checkOutDateType)
            {
                case ClockingHistoryType.DayBefore:
                    return clockingEndTime.Date.AddDays(-1).AddMinutes(
                        checkedOutDate.Value.Hour * 60 + checkedOutDate.Value.Minute);
                case ClockingHistoryType.Today:
                    return clockingEndTime.Date.AddMinutes(
                        checkedOutDate.Value.Hour * 60 + checkedOutDate.Value.Minute);
                case ClockingHistoryType.DayAfter:
                    return clockingEndTime.Date.AddDays(1).AddMinutes(
                        checkedOutDate.Value.Hour * 60 + checkedOutDate.Value.Minute);
                default:
                    //Trường hợp chấm công bằng máy chấm công
                    return checkedOutDate;
            }
        }


        public static ClockingHistory GenerateClockingHistory(
            long clockingId,
            int branchId,
            int tenantId,
            DateTime? checkedInDate,
            DateTime? checkedOutDate,
            DateTime clockingStartTime,
            DateTime clockingEndTime,
            byte timeKeepingType,
            SettingsToObject settings,
            byte clockingStatus,
            int timeIsLate,
            int overTimeBeforeShiftWork,
            int timeIsLeaveWorkEarly,
            int overTimeAfterShiftWork,
            byte? oldAbsenceType,
            byte? absenceType,
            bool leaveOfAbsence,
            long shiftId,
            long employeeId,
            ClockingHistory lastClockingHistory = null,
            DateTime? checkTime = null,
            string autoTimekeepingUid = null,
            ClockingHistoryType? checkInDateType = null,
            ClockingHistoryType? checkOutDateType = null
            )
        {
            ClockingHistory clockingHistory = null;
            if (leaveOfAbsence)
            {
                clockingHistory = GenerateClockingExistsLeaveOfAbsence(clockingId, branchId, tenantId, clockingStartTime, clockingEndTime, timeKeepingType, settings, clockingStatus, timeIsLate, overTimeBeforeShiftWork, timeIsLeaveWorkEarly, overTimeAfterShiftWork, oldAbsenceType, absenceType, shiftId, employeeId, lastClockingHistory, clockingHistory, checkTime, autoTimekeepingUid);
            }
            else
            {
                var isClockingNotCheckInAndNotCheckOut = checkedInDate == null && checkedOutDate == null && clockingStatus > (byte) ClockingStatuses.Created;
                byte newClockingStatus;
                byte timeKeepingTypeValue;
                if (isClockingNotCheckInAndNotCheckOut)
                {
                    newClockingStatus = (byte)ClockingStatuses.Created;
                    timeKeepingTypeValue = GetTimeKeepingTypeValue(timeKeepingType, lastClockingHistory, newClockingStatus);

                    clockingHistory = new ClockingHistory(
                        clockingId,
                        branchId,
                        tenantId,
                        null,
                        null,
                        clockingStartTime,
                        clockingEndTime,
                        timeKeepingTypeValue,
                        settings,
                        newClockingStatus,
                        timeIsLate,
                        overTimeBeforeShiftWork,
                        timeIsLeaveWorkEarly,
                        overTimeAfterShiftWork,
                        shiftId,
                        employeeId,
                        lastClockingHistory,
                        null,
                        checkTime,
                        autoTimekeepingUid,
                        checkInDateType,
                        checkOutDateType
                    );
                    return clockingHistory;
                }
                
                var isTimekeepingType = IsTimekeepingTypeUnAbsence(checkedInDate, checkedOutDate, timeKeepingType,
                    timeIsLate, overTimeBeforeShiftWork, timeIsLeaveWorkEarly, overTimeAfterShiftWork,
                    lastClockingHistory);

                if (!isTimekeepingType) return null;

                if (checkedInDate == null && checkedOutDate == null) return null;

                newClockingStatus = checkedOutDate != null
                    ? (byte)ClockingStatuses.CheckedOut
                    : (byte)ClockingStatuses.CheckedIn;

                timeKeepingTypeValue = GetTimeKeepingTypeValue(timeKeepingType, lastClockingHistory, newClockingStatus);

                clockingHistory = new ClockingHistory(
                    clockingId,
                    branchId,
                    tenantId,
                    checkedInDate,
                    checkedOutDate,
                    clockingStartTime,
                    clockingEndTime,
                    timeKeepingTypeValue,
                    settings,
                    newClockingStatus,
                    timeIsLate,
                    overTimeBeforeShiftWork,
                    timeIsLeaveWorkEarly,
                    overTimeAfterShiftWork,
                    shiftId,
                    employeeId,
                    lastClockingHistory,
                    absenceType,
                    checkTime,
                    autoTimekeepingUid,
                    checkInDateType,
                    checkOutDateType
                );
                
            }

            return clockingHistory;
        }

        private static bool IsTimekeepingTypeUnAbsence(DateTime? checkedInDate, DateTime? checkedOutDate, byte timeKeepingType,
            int timeIsLate, int overTimeBeforeShiftWork, int timeIsLeaveWorkEarly, int overTimeAfterShiftWork,
            ClockingHistory lastClockingHistory)
        {
            var isTimekeepingType =
                (timeKeepingType == (byte)TimeKeepingTypes.Fingerprint ||
                 timeKeepingType == (byte)TimeKeepingTypes.Automation) ||
                IsChange(lastClockingHistory, checkedInDate,
                    checkedOutDate, timeIsLate,
                    overTimeBeforeShiftWork, timeIsLeaveWorkEarly,
                    overTimeAfterShiftWork);
            return isTimekeepingType;
        }

        private static byte GetTimeKeepingTypeValue(byte timeKeepingType, ClockingHistory lastClockingHistory,
            byte newClockingStatus)
        {
            var timeKeepingTypeValue = timeKeepingType;
            if (newClockingStatus == lastClockingHistory?.ClockingStatus &&
                timeKeepingType != (byte)TimeKeepingTypes.Fingerprint && timeKeepingType != (byte)TimeKeepingTypes.Automation)
            {
                timeKeepingTypeValue = (byte)TimeKeepingTypes.Updated;
            }

            return timeKeepingTypeValue;
        }

        private static ClockingHistory GenerateClockingExistsLeaveOfAbsence(long clockingId, int branchId, int tenantId,
            DateTime clockingStartTime, DateTime clockingEndTime, byte timeKeepingType, SettingsToObject settings,
            byte clockingStatus, int timeIsLate, int overTimeBeforeShiftWork, int timeIsLeaveWorkEarly,
            int overTimeAfterShiftWork, byte? oldAbsenceType, byte? absenceType, long shiftId, long employeeId,
            ClockingHistory lastClockingHistory, ClockingHistory clockingHistory, DateTime? checkTime,
            string autoTimekeepingUid)
        {
            if (timeKeepingType == (byte)TimeKeepingTypes.Fingerprint)
            {
                clockingHistory = new ClockingHistory(
                    clockingId,
                    branchId,
                    tenantId,
                    null,
                    null,
                    clockingStartTime,
                    clockingEndTime,
                    (byte)TimeKeepingTypes.Fingerprint,
                    settings,
                    (byte)ClockingStatuses.WorkOff,
                    timeIsLate,
                    overTimeBeforeShiftWork,
                    timeIsLeaveWorkEarly,
                    overTimeAfterShiftWork,
                    shiftId,
                    employeeId,
                    lastClockingHistory,
                    absenceType,
                    checkTime,
                    autoTimekeepingUid
                );

                return clockingHistory;
            }

            if (clockingStatus == (byte)ClockingStatuses.WorkOff && oldAbsenceType == absenceType)
                return clockingHistory;

            var newClockingStatus = (byte)ClockingStatuses.WorkOff;
            clockingHistory = new ClockingHistory(
                clockingId,
                branchId,
                tenantId,
                null,
                null,
                clockingStartTime,
                clockingEndTime,
                newClockingStatus == lastClockingHistory?.ClockingStatus
                    ? (byte)TimeKeepingTypes.Updated
                    : timeKeepingType,
                settings,
                newClockingStatus,
                timeIsLate,
                overTimeBeforeShiftWork,
                timeIsLeaveWorkEarly,
                overTimeAfterShiftWork,
                shiftId,
                employeeId,
                lastClockingHistory,
                absenceType,
                checkTime,
                autoTimekeepingUid
            );


            return clockingHistory;
        }
        #endregion

        #region PRIVATE METHODS



        private static bool IsChange(
            ClockingHistory oldClockingHistory,
            DateTime? checkedInDate,
            DateTime? checkedOutDate,
            int timeIsLate,
            int overTimeBeforeShiftWork,
            int timeIsLeaveWorkEarly,
            int overTimeAfterShiftWork)
        {
            if (oldClockingHistory == null)
                return true;

            if (oldClockingHistory.CheckedInDate != checkedInDate)
                return true;

            if (oldClockingHistory.CheckedOutDate != checkedOutDate)
                return true;

            if (oldClockingHistory.TimeIsLate != timeIsLate)
                return true;

            if (oldClockingHistory.OverTimeBeforeShiftWork != overTimeBeforeShiftWork)
                return true;

            if (oldClockingHistory.TimeIsLeaveWorkEarly != timeIsLeaveWorkEarly)
                return true;

            if (oldClockingHistory.OverTimeAfterShiftWork != overTimeAfterShiftWork)
                return true;

            return false;
        }



        #endregion
    }
}
