using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class BranchSettingTests
    {
        [Fact]
        public void Branch_Setting_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            List<byte> workingDays = new List<byte>(){1, 2 , 3, 4};

            // Act
            var newBranchSetting = fakeBranchSetting();

            // Assert
            Assert.Equal(workingDays, newBranchSetting.WorkingDaysInArray);
        }

        [Fact]
        public void Branch_Setting_Should_Be_Updated_When_Do_Change_Working_Days()
        {
            // Arrange
            var branchSetting = new BranchSetting();
            List<byte> workingDays = new List<byte>() { 1, 2, 3, 4 };

            // Act
            branchSetting.Update(workingDays);

            // Assert
            Assert.Equal(workingDays, branchSetting.WorkingDaysInArray);
        }

        [Fact]
        public void Branch_Setting_Set_Default()
        {
            // Arrange
            var branchSetting = new BranchSetting();

            // Act
            branchSetting.SetDefaultBranchSetting();

            // Assert
            Assert.NotNull(branchSetting);
        }

        [Fact]
        public void Branch_Setting_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var branchSetting = fakeBranchSetting();

            // Assert
            Assert.Contains(branchSetting.DomainEvents, o => o.GetType() == typeof(CreatedBranchSettingEvent));
        }

        [Fact]
        public void Branch_Setting_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var branchSetting = new BranchSetting();
            List<byte> workingDays = new List<byte>() { 1, 2, 3, 4 };

            // Act
            branchSetting.Update(workingDays);

            // Assert
            Assert.Contains(branchSetting.DomainEvents, o => o.GetType() == typeof(UpdatedBranchSettingEvent));
        }

        private BranchSetting fakeBranchSetting()
        {
            List<byte> workingDays = new List<byte>() { 1, 2, 3, 4 };
            var branchSetting = new BranchSetting(2, workingDays);
            return branchSetting;
        }
    }
}
