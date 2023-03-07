using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class ShiftTests
    {
        [Fact]
        public void Shift_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            var shiftName = "Ca sáng";

            // Act
            var shift = fakeShift();

            // Assert
            Assert.Contains(shiftName, shift.Name);
        }

        [Fact]
        public void Shift_Should_Be_Updated_When_Change_Data()
        {
            // Arrange
            var newShift = fakeShift();

            // Act
            var shift = new Shift();
            shift.Update(newShift.Name, newShift.From, newShift.To, newShift.IsActive);

            // Assert
            Assert.Equal(newShift.Name, shift.Name);
        }

        [Fact]
        public void Shift_Should_Be_Updated_When_Delete()
        {
            // Arrange
            var shift = new Shift();
            var IsDelete = true;

            // Act
            shift.Delete();

            // Assert
            Assert.Equal(IsDelete, shift.IsDeleted);
        }

        [Fact]
        public void Shift_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var shift = fakeShift();

            // Assert
            Assert.Contains(shift.DomainEvents, o => o.GetType() == typeof(CreatedShiftEvent));
        }

        [Fact]
        public void Shift_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var newShift = fakeShift();

            // Act
            var shift = new Shift();
            shift.Update(newShift.Name, newShift.From, newShift.To, newShift.IsActive);

            // Assert
            Assert.Contains(shift.DomainEvents, o => o.GetType() == typeof(UpdatedShiftEvent));
        }

        [Fact]
        public void Shift_Must_Have_Send_Delete_Domain_Event()
        {
            // Arrange
            var shift = new Shift();

            // Act
            shift.Delete();

            // Assert
            Assert.Contains(shift.DomainEvents, o => o.GetType() == typeof(DeletedShiftEvent));
        }

        private Shift fakeShift()
        {
            var shift = new Shift("Ca sáng", 10, 20, true, 2);
            return shift;
        }
    }
}
