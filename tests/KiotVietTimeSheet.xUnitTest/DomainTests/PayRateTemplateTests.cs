using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class PayRateTemplateTests
    {
        [Fact]
        public void Pay_Rate_Template_Should_Be_Created_When_Do_Create()
        {
            // Act
            var payRateTemplate = fakePayRateTemplate();

            // Assert
            Assert.NotNull(payRateTemplate);
        }

        [Fact]
        public void Pay_Rate_Template_Should_Be_Updated_When_Do_Update()
        {
            // Act
            List<IRule> rules = new List<IRule>();
            var payRateTemplate = fakePayRateTemplate();

            payRateTemplate.Update(payRateTemplate.Name, payRateTemplate.SalaryPeriod, rules);

            // Assert
            Assert.NotNull(payRateTemplate);
        }

        [Fact]
        public void Pay_Rate_Template_Should_Be_Copied_When_Do_Copy()
        {
            // Arrange
            var copyTemplate = new PayRateTemplate();
            var copyTemplateName = "Nhân viên chính thức";
            var newPayRateTemplate = fakePayRateTemplate();

            // Act
            copyTemplate.Copy(copyTemplateName, newPayRateTemplate);

            Assert.Equal(copyTemplateName, copyTemplate.Name);
        }

        private PayRateTemplate fakePayRateTemplate()
        {
            var payRateTemplate = new PayRateTemplate("Nhân viên thử việc", 1, new List<IRule>(), 2);
            return payRateTemplate;
        }
    }
}
