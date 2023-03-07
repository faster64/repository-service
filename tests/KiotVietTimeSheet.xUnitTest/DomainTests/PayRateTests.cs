using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class PayRateTests
    {
        [Fact]
        public void Pay_Rate_Should_Be_Created_When_Do_Create()
        {
            // Act
            var payRate = fakePayRate();

            // Assert
            Assert.NotNull(payRate);
        }

        [Fact]
        public void Pay_Rate_Should_Be_Updated_When_Do_Update()
        {
            // Act
            List<IRule> rules = new List<IRule>();
            var payRate = fakePayRate();

            payRate.Update(payRate.PayRateTemplateId, payRate.SalaryPeriod, rules);

            // Assert
            Assert.NotNull(payRate);
        }

        [Fact]
        public void Pay_Rate_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var payRate = fakePayRate();

            // Assert
            Assert.Contains(payRate.DomainEvents, o => o.GetType() == typeof(CreatedPayRateEvent));
        }

        [Fact]
        public void Pay_Rate_Must_Have_Send_Update_Domain_Event()
        {
            // Act
            List<IRule> rules = new List<IRule>();
            var payRate = fakePayRate();

            payRate.Update(payRate.PayRateTemplateId, payRate.SalaryPeriod, rules);

            // Assert
            Assert.Contains(payRate.DomainEvents, o => o.GetType() == typeof(UpdatedPayRateEvent));
        }

        private PayRate fakePayRate()
        {
            var payRate = new PayRate(2, null, 1, new List<IRule>());
            return payRate;
        }
    }
}
