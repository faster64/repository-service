using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.DepartmentAggregate.Models;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainTests
{
    public class DepartmentTests
    {
        [Fact]
        public void Department_Should_Be_Created_When_Do_Create()
        {
            // Arrange
            var departmentName = "DEV 1";

            // Act
            var department = fakeDepartment();

            // Assert
            Assert.Contains(departmentName, department.Name);
        }

        [Fact]
        public void Department_Should_Be_Updated_When_Change_Data()
        {
            // Arrange
            var newDepartment = fakeDepartment();

            // Act
            var department = new Department();
            department.Update(newDepartment.Name, newDepartment.Description, newDepartment.IsActive);

            // Assert
            Assert.Equal(newDepartment.Name, department.Name);
        }

        [Fact]
        public void Department_Should_Be_Updated_When_Delete()
        {
            // Arrange
            var department = new Department();
            var IsDelete = true;

            // Act
            department.Delete();

            // Assert
            Assert.Equal(IsDelete, department.IsDeleted);
        }

        [Fact]
        public void Department_Must_Have_Send_Create_Domain_Event()
        {
            // Act
            var department = fakeDepartment();

            // Assert
            Assert.Contains(department.DomainEvents, o => o.GetType() == typeof(CreatedDepartmentEvent));
        }

        [Fact]
        public void Department_Must_Have_Send_Update_Domain_Event()
        {
            // Arrange
            var newDepartment = fakeDepartment();

            // Act
            var department = new Department();
            department.Update(newDepartment.Name, newDepartment.Description, newDepartment.IsActive);

            // Assert
            Assert.Contains(department.DomainEvents, o => o.GetType() == typeof(UpdatedDepartmentEvent));
        }

        [Fact]
        public void Department_Must_Have_Send_Delete_Domain_Event()
        {
            // Arrange
            var department = new Department();

            // Act
            department.Delete();

            // Assert
            Assert.Contains(department.DomainEvents, o => o.GetType() == typeof(DeletedDepartmentEvent));
        }

        private Department fakeDepartment()
        {
            var department = new Department("DEV 1", "Dev 1", true);
            return department;
        }
    }
}
