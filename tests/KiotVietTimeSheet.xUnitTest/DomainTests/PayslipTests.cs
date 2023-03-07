using System;
using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class PayslipTests
    {
        [Fact]
        public void Payslip_Should_Be_Created_When_Do_Create()
        {
            // Act
            var payslip = fakePayslip();

            // Assert
            Assert.NotNull(payslip);
        }

        [Fact]
        public void Payslip_Should_Be_Updated_Status_To_Void_When_Do_Cancel()
        {
            // Arrange
            var payslip = fakePayslip();
            var expectedPayslipStatus = PayslipStatuses.Void;

            // Act
            payslip.Cancel();

            // Assert
            Assert.Equal((byte)expectedPayslipStatus, payslip.PayslipStatus);
        }

        [Fact]
        public void Payslip_Should_Be_Deleted_When_Do_Cancel()
        {
            // Arrange
            var payslip = fakePayslip();
            payslip.IsDraft = true;

            var IsDelete = true;

            // Act
            payslip.Cancel();

            // Assert
            Assert.Equal(IsDelete, payslip.IsDeleted);
        }

        [Fact]
        public void Payslip_Must_Have_Send_Cancel_Domain_Event()
        {
            // Arrange
            var payslip = fakePayslip();

            // Act
            payslip.Cancel();

            // Assert
            Assert.Contains(payslip.DomainEvents, o => o.GetType() == typeof(CancelPayslipEvent));
        }

        [Fact]
        public void Payslip_Should_Be_Updated_Status_When_Do_Cancel_Without_Event()
        {
            // Arrange
            var payslip = fakePayslip();
            var expectedPayslipStatus = PayslipStatuses.Void;

            // Act
            payslip.CancelWithoutEvent();

            // Assert
            Assert.Equal((byte)expectedPayslipStatus, payslip.PayslipStatus);
        }

        [Fact]
        public void Paysheet_Id_Should_Be_Updated_When_Do_Update()
        {
            // Arrange
            var payslip = fakePayslip();
            var expectedPaysheetId = 2;

            // Act
            payslip.UpdatePaysheetId(expectedPaysheetId);

            // Assert
            Assert.Equal(expectedPaysheetId, payslip.PaysheetId);
        }

        [Fact]
        public void Payslip_Should_Be_Updated_Status_To_Paid_When_Do_Complete()
        {
            // Arrage
            var payslip = fakePayslip();
            var expectedPayslipStatus = PayslipStatuses.PaidSalary;

            // Act
            payslip.Complete();

            // Assert
            Assert.Equal((byte)expectedPayslipStatus, payslip.PayslipStatus);
        }

        [Fact]
        public void Payslip_Should_Be_Update_Total_Payment_When_Do_Update_Amount()
        {
            // Arranfe
            var payslip = fakePayslip();
            var expectedTotalPayment = 10000;

            // Act
            payslip.UpdateTotalPaymentWithAmout(expectedTotalPayment);

            // Assert
            Assert.Equal(expectedTotalPayment, payslip.TotalPayment);
        }

        [Fact]
        public void Payslip_Should_Be_Update_Total_Payment_When_Do_Update()
        {
            // Arranfe
            var payslip = fakePayslip();
            var expectedTotalPayment = 10000;

            // Act
            payslip.UpdateTotalPayment(expectedTotalPayment);

            // Assert
            Assert.Equal(expectedTotalPayment, payslip.TotalPayment);
        }

        private Payslip fakePayslip()
        {
            var payslip = new Payslip(1, 1, 100000, 10000, 0, 0, 0, 0, (byte) PayslipStatuses.Draft, "PL000001", "Note",
                1, DateTime.Now, new List<IRule>());

            return payslip;
        }
    }
}
