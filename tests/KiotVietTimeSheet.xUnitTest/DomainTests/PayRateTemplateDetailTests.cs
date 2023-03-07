using KiotVietTimeSheet.Domain.AggregatesModels.PayRateTemplateAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class PayRateTemplateDetailTests
    {
        [Fact]
        public void Pay_Rate_Template_Detail_Should_Be_Created_When_Do_Create()
        {
            // Act
            var payRateTemplateDetail = fakePayRateTemplateDetail();

            // Assert
            Assert.NotNull(payRateTemplateDetail);
        }

        [Fact]
        public void Pay_Rate_Template_Detail_Should_Be_Updated_When_Do_Update()
        {
            var ruleValue = "{\"AllowanceRuleValueDetails\":[{\"AllowanceId\":1,\"Name\":null,\"Value\":3000.0,\"ValueRatio\":null,\"Rank\":0}]}";
            // Act
            var payRateTemplateDetail = fakePayRateTemplateDetail();

            payRateTemplateDetail.Update(ruleValue);

            // Assert
            Assert.Equal(ruleValue, payRateTemplateDetail.RuleValue);
        }

        private PayRateTemplateDetail fakePayRateTemplateDetail()
        {
            var payRateTemplateDetail = new PayRateTemplateDetail(1, "MainSalaryRule", "{\"Type\":2,\"MainSalaryValueDetails\":[{\"ShiftId\":0,\"Default\":300000.0,\"DayOff\":100.0,\"Holiday\":100.0,\"Rank\":0}]}");
            return payRateTemplateDetail;
        }
    }
}
