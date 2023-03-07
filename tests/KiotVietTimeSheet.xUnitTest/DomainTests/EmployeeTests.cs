using System.Collections.Generic;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class EmployeeTests
    {
        [Fact]
        public void Employee_Should_Be_Created_When_Do_Create()
        {
            // Act
            var employee = fakeEmployee();

            // Assert
            Assert.NotNull(employee);
        }

        [Fact]
        public void Employee_Should_Be_Updated_When_Do_Update()
        {
            // Arrange
            var newEmployee = fakeEmployee();

            // Act
            var employee = new Employee();
            employee.Update(newEmployee.Code, newEmployee.Name, newEmployee.DOB, newEmployee.Gender,
                newEmployee.IdentityNumber, newEmployee.MobilePhone, newEmployee.Email, newEmployee.Facebook,
                newEmployee.Address, newEmployee.LocationName, newEmployee.WardName, newEmployee.Note,
                newEmployee.DepartmentId, newEmployee.JobTitleId, newEmployee.ProfilePictures, newEmployee.UserId);

            // Assert
            Assert.Equal(newEmployee.Code, employee.Code);
        }

        [Fact]
        public void Employee_Should_Be_Updated_User_Id_When_Do_Update()
        {
            // Arrange
            var employee = new Employee();
            var userId = 10;

            // Act
            employee.UpdateUserId(userId);

            // Assert
            Assert.Equal(userId, employee.UserId);
        }

        [Fact]
        public void Employee_Should_Be_Updated_Profile_Picture_When_Do_Update()
        {
            // Arrange
            var employee = new Employee();
            List<EmployeeProfilePicture> profilePictures = null;

            // Act
            employee.UpdateProfilePicture();

            // Assert
            Assert.Equal(profilePictures, employee.ProfilePictures);
        }

        [Fact]
        public void Employee_Should_Be_Updated_User_When_Do_UnAssign_User()
        {
            // Arrange
            var employee = new Employee();
            var userId = (object)null;

            // Act
            employee.UnAssignUser();

            // Assert
            Assert.Equal(userId, employee.UserId);
        }

        [Fact]
        public void Employee_Should_Be_Updated_Department_When_Do_Left_Department()
        {
            // Arrange
            var employee = new Employee();
            var departmentId = (object)null;

            // Act
            employee.LeftDeparment();

            // Assert
            Assert.Equal(departmentId, employee.DepartmentId);
        }

        [Fact]
        public void Employee_Should_Be_Updated_Job_Title_When_Do_Left_Job_Title()
        {
            // Arrange
            var employee = new Employee();
            var jobTitleId = (object)null;

            // Act
            employee.LeftJobTitle();

            // Assert
            Assert.Equal(jobTitleId, employee.JobTitleId);
        }

        [Fact]
        public void Employee_Should_Be_Updated_When_Delete()
        {
            // Arrange
            var employee = new Employee();
            var IsDelete = true;

            // Act
            employee.Delete();

            // Assert
            Assert.Equal(IsDelete, employee.IsDeleted);
        }

        [Fact]
        public void Employee_Must_Have_Send_Create_Domain_Event()
        {
            // Arrange
            var employee = new Employee();

            // Act
            employee.Delete();

            // Assert
            Assert.Contains(employee.DomainEvents, o => o.GetType() == typeof(DeletedEmployeeEvent));
        }

        private Employee fakeEmployee()
        {
            var employee = new Employee();

            return employee;
        }
    }
}
