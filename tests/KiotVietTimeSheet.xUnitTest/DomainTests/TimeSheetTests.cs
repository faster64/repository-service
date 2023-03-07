using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class TimeSheetTests
    {
        [Fact]
        public void Time_Sheet_Should_Be_Created_When_Do_Create()
        {
            // Act
            var timeSheet = fakeTimeSheet();

            // Assert
            Assert.NotNull(timeSheet);
        }

        [Fact]
        public void Time_Sheet_Should_Be_Updated_When_Do_Update()
        {
            // Arrange
            var newTimeSheet = fakeTimeSheet();

            // Act
            var timeSheet = new TimeSheet();
            timeSheet.Update(newTimeSheet.StartDate, newTimeSheet.IsRepeat, newTimeSheet.RepeatType,
                newTimeSheet.RepeatEachDay, newTimeSheet.EndDate, newTimeSheet.SaveOnDaysOffOfBranch,
                newTimeSheet.SaveOnHoliday, newTimeSheet.Note, new List<TimeSheetShift>());

            // Assert
            Assert.NotNull(timeSheet);
        }

        [Fact]
        public void Time_Sheet_Should_Be_Updated_When_Do_Cancel()
        {
            // Arrange
            var timeSheet = new TimeSheet();
            var timeSheetStatus = (byte) TimeSheetStatuses.Void;

            // Act
            timeSheet.Cancel();

            // Assert
            Assert.Equal(timeSheetStatus, timeSheet.TimeSheetStatus);
        }

        [Fact]
        public void Time_Sheet_Status_Should_Be_Updated_When_Do_Update()
        {
            // Arrange
            var timeSheet = new TimeSheet();
            var timeSheetStatus = (byte)TimeSheetStatuses.Created;

            // Act
            timeSheet.UpdateTimeSheetStatus(timeSheetStatus);

            // Assert
            Assert.Equal(timeSheetStatus, timeSheet.TimeSheetStatus);
        }

        [Fact]
        public void Time_Sheet_Should_Be_Cloned_When_Do_Clone()
        {
            // Arrange
            var newTimeSheet = fakeTimeSheet();
            var timeSheet = new TimeSheet();

            // Act
            var cloneTimeSheet = timeSheet.CloneTimeSheet(newTimeSheet);

            // Assert
            Assert.Equal(newTimeSheet.BranchId, cloneTimeSheet.BranchId);
        }

        [Fact]
        public void Time_Sheet_Should_Be_Updated_EmployeeId_When_Do_Update()
        {
            // Arrange
            var timeSheet = new TimeSheet();
            var employeeId = 10;

            // Act
            timeSheet.SetEmployeeId(employeeId);

            // Assert
            Assert.Equal(employeeId, timeSheet.EmployeeId);
        }

        [Fact]
        public void Time_Sheet_Should_Be_Updated_Repeat_Each_Day_When_Do_Update()
        {
            // Arrange
            var timeSheet = new TimeSheet();
            var repeatEachDay = 2;

            // Act
            timeSheet.SetRepeatEachDay((byte)repeatEachDay);

            // Assert
            Assert.Equal((byte)repeatEachDay, timeSheet.RepeatEachDay);
        }

        [Fact]
        public void Time_Sheet_Should_Be_Copied_When_Do_Copy()
        {
            // Arrange
            var newTimeSheet = fakeTimeSheet();
            var timeSheet = new TimeSheet();

            // Act
            timeSheet.CopyTimeSheet(newTimeSheet.Id, newTimeSheet.StartDate, newTimeSheet.EndDate, newTimeSheet.TimeSheetShifts);

            // Assert
            Assert.Equal(newTimeSheet.StartDate, timeSheet.StartDate);
        }

        [Fact]
        public void Time_Sheet_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var timeSheet = fakeTimeSheet();

            // Assert
            Assert.Contains(timeSheet.DomainEvents, o => o.GetType() == typeof(CreatedTimeSheetEvent));
        }

        [Fact]
        public void Time_Sheet_Must_Have_Send_Create_Domain_Event_When_Do_Copy()
        {
            // Arrange
            var newTimeSheet = fakeTimeSheet();
            var timeSheet = new TimeSheet();

            // Act
            timeSheet.CopyTimeSheet(newTimeSheet.Id, newTimeSheet.StartDate, newTimeSheet.EndDate, newTimeSheet.TimeSheetShifts);

            // Assert
            Assert.Contains(timeSheet.DomainEvents, o => o.GetType() == typeof(CreatedTimeSheetEvent));
        }

        [Fact]
        public void Time_Sheet_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var newTimeSheet = fakeTimeSheet();

            // Act
            var timeSheet = new TimeSheet();
            timeSheet.Update(newTimeSheet.StartDate, newTimeSheet.IsRepeat, newTimeSheet.RepeatType,
                newTimeSheet.RepeatEachDay, newTimeSheet.EndDate, newTimeSheet.SaveOnDaysOffOfBranch,
                newTimeSheet.SaveOnHoliday, newTimeSheet.Note, new List<TimeSheetShift>());

            // Assert
            Assert.Contains(timeSheet.DomainEvents, o => o.GetType() == typeof(UpdatedTimeSheetEvent));
        }

        [Fact]
        public void Time_Sheet_Must_Have_Send_Cancel_Domain_Event()
        {
            // Arrange
            var timeSheet = new TimeSheet();

            // Act
            timeSheet.Cancel();

            // Assert
            Assert.Contains(timeSheet.DomainEvents, o => o.GetType() == typeof(CancelTimeSheetEvent));
        }

        private TimeSheet fakeTimeSheet()
        {
            var timeSheet = new TimeSheet(1, DateTime.Now, false, 1, 1, DateTime.Now, 2, false, false,
                new List<TimeSheetShift>(), "");
            return timeSheet;
        }
    }
}
