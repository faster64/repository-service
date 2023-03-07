using System;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class HolidayTests
    {
        [Fact]
        public void Holiday_Should_Be_Created_When_Do_Create()
        {
            // Act
            var holiday = fakeHoliday();

            // Assert
            Assert.NotNull(holiday);
        }

        [Fact]
        public void Holiday_Should_Be_Updated_When_Do_Update()
        {
            // Arrange
            var newHoliday = fakeHoliday();

            // Act
            var holiday = new Holiday();
            holiday.Update(newHoliday.Name, newHoliday.From, newHoliday.To);

            // Assert
            Assert.Equal(newHoliday.Name, holiday.Name);
        }

        [Fact]
        public void Holiday_Should_Be_Updated_When_Delete()
        {
            // Arrange
            var holiday = new Holiday();
            var IsDelete = true;

            // Act
            holiday.Delete();

            // Assert
            Assert.Equal(IsDelete, holiday.IsDeleted);
        }

        [Fact]
        public void Holiday_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var holiday = fakeHoliday();

            // Assert
            Assert.Contains(holiday.DomainEvents, o => o.GetType() == typeof(CreatedHolidayEvent));
        }

        [Fact]
        public void Holiday_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var newHoliday = fakeHoliday();

            // Act
            var holiday = new Holiday();
            holiday.Update(newHoliday.Name, newHoliday.From, newHoliday.To);

            // Assert
            Assert.Contains(holiday.DomainEvents, o => o.GetType() == typeof(UpdatedHolidayEvent));
        }

        [Fact]
        public void Holiday_Must_Have_Send_Delete_Domain_Event()
        {
            // Arrange
            var holiday = new Holiday();

            // Act
            holiday.Delete();

            // Assert
            Assert.Contains(holiday.DomainEvents, o => o.GetType() == typeof(DeletedHolidayEvent));
        }

        private Holiday fakeHoliday()
        {
            var holiday = new Holiday("Nghỉ lễ 2/9", DateTime.Now, DateTime.Now);
            return holiday;
        }
    }
}
