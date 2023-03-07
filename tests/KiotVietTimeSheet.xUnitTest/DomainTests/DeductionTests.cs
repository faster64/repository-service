using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class DeductionTests
    {
        [Fact]
        public void Deduction_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            var deductionName = "GT 1";

            // Act
            var newDeduction = new Deduction(deductionName);

            // Assert
            Assert.Equal(deductionName, newDeduction.Name);
        }

        [Fact]
        public void Deduction_Should_Be_Updated_When_Do_Change_Name()
        {
            // Arrange
            var deduction = new Deduction();
            var deductionName = "Đi muộn";

            // Act
            deduction.Update(deductionName);

            // Assert
            Assert.Equal(deductionName, deduction.Name);
        }

        [Fact]
        public void Deduction_Should_Be_Updated_When_Do_Delete()
        {
            // Arrange
            var deduction = new Deduction();
            var IsDelete = true;

            // Act
            deduction.Delete();

            // Assert
            Assert.Equal(IsDelete, deduction.IsDeleted);
        }
    }
}
