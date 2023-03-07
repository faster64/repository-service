using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class TimeSheetShiftTests
    {
        [Fact]
        public void Time_Sheet_Shift_Should_Be_Created_When_Do_Create()
        {
            // Act
            var timeSheetShift = fakeTimeSheetShift();

            // Assert
            Assert.NotNull(timeSheetShift);
        }

        [Fact]
        public void Time_Sheet_Shift_Should_Be_Updated_Time_Sheet_Id_When_Do_Update()
        {
            // Arrange
            var timeSheetShift = fakeTimeSheetShift();
            var timeSheetId = 2;

            // Act
            timeSheetShift.UpdateTimeSheetId(timeSheetId);

            // Assert
            Assert.Equal(timeSheetId, timeSheetShift.TimeSheetId);
        }

        private TimeSheetShift fakeTimeSheetShift()
        {
            var timeSheetShift = new TimeSheetShift(1, "1,2", "1");
            return timeSheetShift;
        }
    }
}
