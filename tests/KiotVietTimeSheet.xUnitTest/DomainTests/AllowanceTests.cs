using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class AllowanceTests
    {
        [Fact]
        public void Allowance_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            var allowanceName = "PC 1";

            // Act
            var newAllowance = new Allowance(allowanceName);

            // Assert
            Assert.Equal(allowanceName, newAllowance.Name);
        }

        [Fact]
        public void Allowance_Should_Be_Updated_When_Do_Change_Name()
        {
            // Arrange
            var allowance = new Allowance();
            var allowanceName = "Xăng xe";

            // Act
            allowance.Update(allowanceName);

            // Assert
            Assert.Equal(allowanceName, allowance.Name);
        }

        [Fact]
        public void Allowance_Should_Be_Updated_When_Do_Delete()
        {
            // Arrange
            var allowance = new Allowance();
            var IsDelte = true;

            // Act
            allowance.Delete();

            // Assert
            Assert.Equal(IsDelte, allowance.IsDeleted);
        }
    }
}
