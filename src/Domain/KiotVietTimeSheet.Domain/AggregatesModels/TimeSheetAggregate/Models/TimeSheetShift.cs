﻿using System.Collections.Generic;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using System.Linq;
using Newtonsoft.Json;

namespace KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models
{
    public class TimeSheetShift : BaseEntity,
        IAggregateRoot,
        IEntityIdlong
    {
        public const byte DefaultDailyRepeatDay = 1;

        #region Constructor
        public TimeSheetShift()
        {
        }

        [JsonConstructor]
        public TimeSheetShift(long id, long timeSheetId, string shiftIds, string repeatDaysOfWeek, TimeSheet timeSheet)
        {
            Id = id;
            TimeSheetId = timeSheetId;
            ShiftIds = shiftIds;
            RepeatDaysOfWeek = repeatDaysOfWeek;
        }

        public TimeSheetShift(long timeSheetId, string shiftIds, string repeatDaysOfWeek)
        {
            TimeSheetId = timeSheetId;
            ShiftIds = shiftIds;
            RepeatDaysOfWeek = repeatDaysOfWeek;
        }

        #endregion

        public long Id { get; set; }
        public long TimeSheetId { get; protected set; }
        public string ShiftIds { get; protected set; }
        public string RepeatDaysOfWeek { get; protected set; }


        public List<byte> RepeatDaysOfWeekInList => GetRepeatDaysOfWeekInList();

        public List<long> ShiftIdsToList => GetShiftIdsToList();

        public List<byte> GetRepeatDaysOfWeekInList()
        {
            List<byte> repeateDays = new List<byte>();
            if (string.IsNullOrEmpty(RepeatDaysOfWeek)) return repeateDays;
            return RepeatDaysOfWeek.Split(',').Select(d => byte.Parse(string.IsNullOrEmpty(d) ? "" : d.Trim())).ToList();
        }

        public List<long> GetShiftIdsToList()
        {
            var listTimeSheetShift = ShiftIds.Split(',')
                .Select(d => long.Parse(string.IsNullOrEmpty(d) ? "" : d.Trim())).ToList();
            return string.IsNullOrEmpty(ShiftIds) ? new List<long>() : listTimeSheetShift;
        }

        public void UpdateTimeSheetId(long timeSheetId)
        {
            TimeSheetId = timeSheetId;
        }
    }
}
