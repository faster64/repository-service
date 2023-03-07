using System;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class ClockingTests
    {
        [Fact]
        public void Clocking_Should_Be_Updated_When_Do_Update_Without_Add_Domain_Event()
        {
            // Arrange
            var newClocking = fakeClocking();
            var shift = new Shift();
            var clocking = new Clocking();

            // Act
            clocking.UpdateWithoutAddDomainEvent(newClocking.ShiftId, newClocking.StartTime, newClocking.EndTime, newClocking.Note, shift);

            // Assert
            Assert.Equal(newClocking.ShiftId, clocking.ShiftId);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_Status_When_Do_Update_Status()
        {
            // Arrange
            var clocking = new Clocking();
            var clockingStatus = 1;

            // Act
            clocking.UpdateClockingStatus((byte)clockingStatus);

            // Assert
            Assert.Equal(clockingStatus, clocking.ClockingStatus);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_Time_Sheet_Id_When_Do_Update()
        {
            // Arrange
            var clocking = new Clocking();
            var timeSheetId = 1;

            // Act
            clocking.UpdateClockingTimeSheetId(timeSheetId);

            // Assert
            Assert.Equal(timeSheetId, clocking.TimeSheetId);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_Shift_And_DateTime_When_Do_Update()
        {
            // Arrange
            var clocking = new Clocking();
            var shiftId = 1;
            var startTime = DateTime.Now;
            var shift = new Shift("Ca sáng", 10, 20, true, 2);

            // Act
            clocking.UpdateClockingShiftAndDateTime(shiftId, startTime, shift);

            // Assert
            Assert.Equal(shiftId, clocking.ShiftId);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_Checked_In_Date_When_Do_Update()
        {
            // Arrange
            var clocking = fakeClocking();
            var checkedInDate = DateTime.Now;

            // Act
            clocking.UpdateClockingCheckedInDate(checkedInDate);

            // Assert
            Assert.Equal(checkedInDate, clocking.CheckInDate);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_Checked_Out_Date_When_Do_Update()
        {
            // Arrange
            var clocking = fakeClocking();
            var checkedOutDate = DateTime.Now;

            // Act
            clocking.UpdateClockingCheckedOutDate(checkedOutDate);

            // Assert
            Assert.Equal(checkedOutDate, clocking.CheckOutDate);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_When_Do_Reject()
        {
            // Arrange
            var clocking = new Clocking();
            var clockingStatus = (byte)ClockingStatuses.Void;

            // Act
            clocking.Reject();

            // Assert
            Assert.Equal(clockingStatus, clocking.ClockingStatus);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_When_Delete()
        {
            // Arrange
            var clocking = new Clocking();
            var IsDelete = true;

            // Act
            clocking.Delete();

            // Assert
            Assert.Equal(IsDelete, clocking.IsDeleted);
        }

        [Fact]
        public void Clocking_Should_Be_Copied_When_Do_Copy()
        {
            // Arrange
            var newClocking = fakeClocking();

            // Act
            var clockingCopied =  newClocking.CreateCopy();

            // Assert
            Assert.Equal(newClocking.Id, clockingCopied.Id);
        }

        [Fact]
        public void Clocking_Should_Be_Swapped_When_Do_Swap()
        {
            // Arrange
            var sourceClocking = fakeClocking();
            var targetClocking = fakeClocking2();

            var oldSource = sourceClocking.CreateCopy();
            var oldTarget = targetClocking.CreateCopy();

            // Act
            sourceClocking.SwapShift(oldTarget, oldSource);

            // Assert
            Assert.Equal(sourceClocking.ShiftId, targetClocking.ShiftId);
        }

        [Fact]
        public void Clocking_Should_Be_Updated_Payment_Status_When_Update()
        {
            // Arrange
            var clocking = new Clocking();
            var clockingPaymentStatus = (byte) ClockingPaymentStatuses.Paid;

            // Act
            clocking.UpdateClockingPaymentStatus(clockingPaymentStatus);

            // Assert
            Assert.Equal(clockingPaymentStatus, clocking.ClockingPaymentStatus);
        }

        private Clocking fakeClocking()
        {
            var clocking = new Clocking(1, 1, 10, 10, DateTime.Now, DateTime.Now, 1, 1, "");

            return clocking;
        }

        private Clocking fakeClocking2()
        {
            var clocking = new Clocking(2, 2, 20, 20, DateTime.Now, DateTime.Now, 1, 1, "");

            return clocking;
        }
    }
}
