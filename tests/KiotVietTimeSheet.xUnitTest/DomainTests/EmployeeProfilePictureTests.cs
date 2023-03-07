using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class EmployeeProfilePictureTests
    {
        [Fact]
        public void Employee_Profile_Picture_Should_Be_Created_When_Do_Create()
        {
            // Act
            var employeeProfilePicture = fakeEmployeeProfilePicture();

            // Assert
            Assert.NotNull(employeeProfilePicture);
        }

        private EmployeeProfilePicture fakeEmployeeProfilePicture()
        {
            var employeeProfilePicture = new EmployeeProfilePicture(1, "https://d1g7qs2bqguupl.cloudfront.net/1006/c967650e23264b938c481ff47266ac70", 1, false);

            return employeeProfilePicture;
        }
    }
}
