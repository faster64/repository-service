using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class PayRateDetailTests
    {
        [Fact]
        public void Pay_Rate_Detail_Should_Be_Created_When_Do_Create()
        {
            // Act
            var newPayRateDetail = fakePayRateDetail();

            // Assert
            Assert.NotNull(newPayRateDetail);
        }

        [Fact]
        public void Pay_Rate_Detail_Should_Be_Update_When_Do_Update()
        {
            // Arrange
            var ruleValue = "{\"Type\":2,\"MainSalaryValueDetails\":[{\"ShiftId\":0,\"Default\":10000.0,\"DayOff\":100.0,\"Holiday\":100.0,\"Rank\":0}]}";
            var newPayRateDetail = fakePayRateDetail();

            // Act
            newPayRateDetail.Update(ruleValue);

            // Assert
            Assert.NotNull(newPayRateDetail);
        }

        private PayRateDetail fakePayRateDetail()
        {
            var payRateDetail = new PayRateDetail(1, "{}", "{}", 1);
            return payRateDetail;
        }
    }
}
