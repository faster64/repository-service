using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events;
using Newtonsoft.Json;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models
{
    public class TimeSheet : BaseEntity,
        IAggregateRoot,
        IEntityIdlong,
        ITenantId,
        IBranchId,
        ICreatedBy,
        ICreatedDate,
        IModifiedBy,
        IModifiedDate,
        ISoftDelete
    {
        public const byte DefaultDailyRepeatDay = 1;

        #region Constructor
        public TimeSheet()
        {
            IsDeleted = false;
            SaveOnHoliday = false;
            SaveOnDaysOffOfBranch = false;
        }

        [JsonConstructor]
        public TimeSheet(long id, long employeeId, DateTime startDate, DateTime endDate, bool? isRepeat, byte? repeatType, byte? repeatEachDay, List<Clocking> clockings, int branchId, int tenantId, long createdBy, DateTime createdDate, long? modifiedBy, DateTime? modifiedDate, bool isDeleted, long? deletedBy, DateTime? deletedDate, byte timeSheetStatus, bool saveOnDaysOffOfBranch, bool saveOnHoliday, Employee employee, List<TimeSheetShift> timeSheetShifts, string note, byte autoGenerateClockingStatus)
        {
            Id = id;
            EmployeeId = employeeId;
            StartDate = startDate;
            EndDate = endDate;
            IsRepeat = isRepeat;
            RepeatType = repeatType;
            RepeatEachDay = repeatEachDay;
            BranchId = branchId;
            TenantId = tenantId;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            IsDeleted = isDeleted;
            DeletedBy = deletedBy;
            DeletedDate = deletedDate;
            TimeSheetStatus = timeSheetStatus;
            SaveOnDaysOffOfBranch = saveOnDaysOffOfBranch;
            SaveOnHoliday = saveOnHoliday;
            Employee = employee;
            TimeSheetShifts = timeSheetShifts;
            Note = note;
            AutoGenerateClockingStatus = autoGenerateClockingStatus;
        }

        public TimeSheet(
            long employeeId,
            DateTime startDate,
            bool? isRepeat,
            byte? repeatType,
            byte? repeatEachDay,
            DateTime endDate,
            int branchId,
            bool saveOnDaysOffOfBranch,
            bool saveOnHoliday,
            List<TimeSheetShift> timeSheetShifts,
            string note,
            byte autoGenerateClockingStatus
        )
        {

            EmployeeId = employeeId;
            StartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0);
            IsRepeat = isRepeat;
            RepeatType = repeatType;
            RepeatEachDay = repeatEachDay;
            EndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            BranchId = branchId;
            SaveOnDaysOffOfBranch = saveOnDaysOffOfBranch;
            SaveOnHoliday = saveOnHoliday;
            TimeSheetStatus = (byte)TimeSheetStatuses.Created;
            TimeSheetShifts = timeSheetShifts;
            Note = note;
            AutoGenerateClockingStatus = autoGenerateClockingStatus;
            AddDomainEvent(new CreatedTimeSheetEvent(this));
        }

        // Only copy primitive values
        public TimeSheet(TimeSheet timeSheet)
        {
            Id = timeSheet.Id;
            EmployeeId = timeSheet.EmployeeId;
            StartDate = timeSheet.StartDate;
            EndDate = timeSheet.EndDate;
            IsRepeat = timeSheet.IsRepeat;
            RepeatType = timeSheet.RepeatType;
            RepeatEachDay = timeSheet.RepeatEachDay;
            BranchId = timeSheet.BranchId;
            TenantId = timeSheet.TenantId;
            CreatedBy = timeSheet.CreatedBy;
            CreatedDate = timeSheet.CreatedDate;
            ModifiedBy = timeSheet.ModifiedBy;
            ModifiedDate = timeSheet.ModifiedDate;
            IsDeleted = timeSheet.IsDeleted;
            DeletedBy = timeSheet.DeletedBy;
            DeletedDate = timeSheet.DeletedDate;
            TimeSheetStatus = timeSheet.TimeSheetStatus;
            SaveOnDaysOffOfBranch = timeSheet.SaveOnDaysOffOfBranch;
            SaveOnHoliday = timeSheet.SaveOnHoliday;
            Note = timeSheet.Note;
            AutoGenerateClockingStatus = timeSheet.AutoGenerateClockingStatus;
            TimeSheetShifts = timeSheet.TimeSheetShifts;

            if (timeSheet.DomainEvents != null)
            {
                foreach (var domainEvent in timeSheet.DomainEvents)
                {
                    AddDomainEvent(domainEvent);
                }
            }
        }

        #endregion

        public long Id { get; set; }
        public long EmployeeId { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public bool? IsRepeat { get; protected set; }
        public byte? RepeatType { get; protected set; }
        public byte? RepeatEachDay { get; protected set; }
        public int BranchId { get; set; }
        public int TenantId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public byte TimeSheetStatus { get; protected set; }
        public bool SaveOnDaysOffOfBranch { get; protected set; }
        public bool SaveOnHoliday { get; protected set; }
        public string Note { get; protected set; }
        public Employee Employee { get; set; }
        public List<TimeSheetShift> TimeSheetShifts { get; set; }
        public long TemporaryId { get; set; }
        public byte AutoGenerateClockingStatus { get; set; }


        public void Update(
            DateTime startDate,
            bool? isRepeat,
            byte? repeatType,
            byte? repeatEachDay,
            DateTime endDate,
            bool saveOnDaysOffOfBranch,
            bool saveOnHoliday,
            string note,
            byte autoGenerateClockingStatus,
            List<TimeSheetShift> timeSheetShifts
        )
        {
            var oldTimeSheet = (TimeSheet)MemberwiseClone();
            StartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0);
            IsRepeat = isRepeat;
            RepeatType = repeatType;
            RepeatEachDay = repeatEachDay;
            EndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            SaveOnDaysOffOfBranch = saveOnDaysOffOfBranch;
            SaveOnHoliday = saveOnHoliday;
            Note = note;
            AutoGenerateClockingStatus = autoGenerateClockingStatus;
            TimeSheetShifts = timeSheetShifts;
            AddDomainEvent(new UpdatedTimeSheetEvent(oldTimeSheet, this));
        }

        public void Cancel()
        {
            var oldTimeSheet = (TimeSheet)MemberwiseClone();
            TimeSheetStatus = (byte)TimeSheetStatuses.Void;
            AddDomainEvent(new CancelTimeSheetEvent(oldTimeSheet));
        }

        /// <summary>
        /// Update status of time sheet
        /// </summary>
        /// <param name="timeSheetStatus"></param>
        public void UpdateTimeSheetStatus(byte timeSheetStatus)
        {
            TimeSheetStatus = timeSheetStatus;
        }

        public TimeSheet CloneTimeSheet(TimeSheet timeSheet)
        {
            return new TimeSheet
            {
                EmployeeId = timeSheet.EmployeeId,
                StartDate = timeSheet.StartDate,
                IsRepeat = timeSheet.IsRepeat,
                RepeatType = timeSheet.RepeatType,
                RepeatEachDay = timeSheet.RepeatEachDay,
                EndDate = timeSheet.EndDate,
                BranchId = timeSheet.BranchId,
                SaveOnDaysOffOfBranch = timeSheet.SaveOnDaysOffOfBranch,
                SaveOnHoliday = timeSheet.SaveOnHoliday,
                TimeSheetStatus = timeSheet.TimeSheetStatus,
                AutoGenerateClockingStatus = timeSheet.AutoGenerateClockingStatus,
                TimeSheetShifts = new List<TimeSheetShift>(timeSheet.TimeSheetShifts),
                Note = timeSheet.Note
            };
        }


        public void SetEmployeeId(long employeeId)
        {
            EmployeeId = employeeId;
        }

        public void SetStartDate(DateTime startDate)
        {
            StartDate = startDate;
        }
        public void SetEndDate(DateTime endDate)
        {
            EndDate = endDate;
        }

        public void SetRepeatEachDay(byte repeatEachDay)
        {
            RepeatEachDay = repeatEachDay;

        }
        public void CopyTimeSheet(
            long id,
            DateTime startDate,
            DateTime endDate,
            List<TimeSheetShift> timeSheetShifts
        )
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            TimeSheetShifts = timeSheetShifts;
            AddDomainEvent(new CreatedTimeSheetEvent(this));
        }

        public void SetRepeatWithDate(bool isRepeat, DateTime startDate, DateTime endDate)
        {
            IsRepeat = isRepeat;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
