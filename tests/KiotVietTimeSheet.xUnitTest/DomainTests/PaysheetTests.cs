using System;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Enum;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class PaysheetTests
    {
        [Fact]
        public void Paysheet_Should_Be_Created_When_Do_Create()
        {
            // Act
            var paysheet = fakePaysheet();

            // Assert
            Assert.NotNull(paysheet);
        }

        [Fact]
        public void Paysheet_Should_Be_Updated_When_Do_Update()
        {
            // Arrange
            var newPaysheet = fakePaysheet();

            // Act
            var paysheet = new Paysheet();
            paysheet.Update(newPaysheet.Code, newPaysheet.Name, newPaysheet.Note, newPaysheet.WorkingDayNumber,
                newPaysheet.SalaryPeriod, newPaysheet.StartTime, newPaysheet.EndTime, newPaysheet.PaysheetPeriodName,
                newPaysheet.CreatorBy, newPaysheet.PaysheetCreatedDate, newPaysheet.PaysheetStatus);

            // Assert
            Assert.Equal(newPaysheet.Code, paysheet.Code);
        }

        [Fact]
        public void Paysheet_Should_Be_Updated_When_Do_Cancel()
        {
            // Arrange
            var paysheet = new Paysheet();
            var paysheetStatus =  (byte)PaysheetStatuses.Void;

            // Act
            paysheet.Cancel();

            // Assert
            Assert.Equal(paysheetStatus, paysheet.PaysheetStatus);
        }

        [Fact]
        public void Paysheet_Should_Be_Updated_When_Do_Complete()
        {
            // Arrange
            var paysheet = new Paysheet();
            var paysheetStatus = (byte)PaysheetStatuses.PaidSalary;

            // Act
            paysheet.Complete();

            // Assert
            Assert.Equal(paysheetStatus, paysheet.PaysheetStatus);
        }

        [Fact]
        public void Paysheet_Must_Have_Send_Cancel_Domain_Event()
        {
            // Arrange
            var paysheet = new Paysheet();

            // Act
            paysheet.Cancel();

            // Assert
            Assert.Contains(paysheet.DomainEvents, o => o.GetType() == typeof(CancelPaysheetEvent));
        }

        private Paysheet fakePaysheet()
        {
            var paysheet = new Paysheet("BL00001", "", 23, 1, DateTime.Now, DateTime.Now.AddDays(23), 1, 1, true, 8);
            return paysheet;
        }
    }
}
