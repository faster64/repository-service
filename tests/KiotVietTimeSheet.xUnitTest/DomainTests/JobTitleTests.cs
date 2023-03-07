using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.JobTitleAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class JobTitleTests
    {
        [Fact]
        public void JobTitle_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            var jobTitleName = "DEV";

            // Act
            var jobTitle = fakeJobTitle();

            // Assert
            Assert.Contains(jobTitleName, jobTitle.Name);
        }

        [Fact]
        public void JobTitle_Should_Be_Updated_When_Change_Data()
        {
            // Arrange
            var newJobTitle = fakeJobTitle();

            // Act
            var jobTitle = new JobTitle();
            jobTitle.Update(newJobTitle.Name, newJobTitle.Description, newJobTitle.IsActive);

            // Assert
            Assert.Equal(newJobTitle.Name, jobTitle.Name);
        }

        [Fact]
        public void JobTitle_Should_Be_Updated_When_Delete()
        {
            // Arrange
            var jobTitle = new JobTitle();
            var IsDelete = true;

            // Act
            jobTitle.Delete();

            // Assert
            Assert.Equal(IsDelete, jobTitle.IsDeleted);
        }

        [Fact]
        public void JobTitle_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var jobTitle = fakeJobTitle();

            // Assert
            Assert.Contains(jobTitle.DomainEvents, o => o.GetType() == typeof(CreatedJobTitleEvent));
        }

        [Fact]
        public void JobTitle_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var newJobTitle = fakeJobTitle();

            // Act
            var jobTitle = new JobTitle();
            jobTitle.Update(newJobTitle.Name, newJobTitle.Description, newJobTitle.IsActive);

            // Assert
            Assert.Contains(jobTitle.DomainEvents, o => o.GetType() == typeof(UpdatedJobTitleEvent));
        }

        [Fact]
        public void JobTitle_Must_Have_Send_Delete_Domain_Event()
        {
            // Arrange
            var jobTitle = new JobTitle();

            // Act
            jobTitle.Delete();

            // Assert
            Assert.Contains(jobTitle.DomainEvents, o => o.GetType() == typeof(DeletedJobTitleEvent));
        }

        private JobTitle fakeJobTitle()
        {
            var jobTitle = new JobTitle("DEV", "Developer", true);
            return jobTitle;
        }
    }
}
