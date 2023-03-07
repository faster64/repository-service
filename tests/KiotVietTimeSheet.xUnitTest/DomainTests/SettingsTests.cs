using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class SettingsTests
    {
        [Fact]
        public void Settings_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            var settingsKey = "EarlyTime";

            // Act
            var settings = fakeSettings();

            // Assert
            Assert.Equal(settingsKey, settings.Name);
        }

        [Fact]
        public void Settings_Should_Be_Updated_When_Change_Data()
        {
            // Arrange
            var newSettingsValue = "1";

            // Act
            var settings = new Settings();
            settings.Update(newSettingsValue);

            // Assert
            Assert.Equal(newSettingsValue, settings.Value);
        }

        [Fact]
        public void Settings_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var settings = fakeSettings();

            // Assert
            Assert.Contains(settings.DomainEvents, o => o.GetType() == typeof(CreatedSettingsEvent));
        }

        [Fact]
        public void Settings_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var newSettings = fakeSettings();

            // Act
            var settings = new Settings();
            settings.Update(newSettings.Value);

            // Assert
            Assert.Contains(settings.DomainEvents, o => o.GetType() == typeof(UpdateSettingsEvent));
        }

        private Settings fakeSettings()
        {
            var settings = new Settings(1, "EarlyTime", "0");
            return settings;
        }
    }
}
