using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Validators.TimeSheetValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using ServiceStack;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class GenerateClockingsDomainServiceUnitTest
    {
        private readonly GenerateClockingsDomainService _generateClockingsDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public GenerateClockingsDomainServiceUnitTest()
        {
            var moqMapper = new Mock<IMapper>();
            var moqBranchSettingReadOnlyRepository = new Mock<IBranchSettingReadOnlyRepository>();
            var moqHolidayReadOnlyRepository = new Mock<IHolidayReadOnlyRepository>();
            var moqShiftReadOnlyRepository = new Mock<IShiftReadOnlyRepository>();
            var moqClockingReadOnlyRepository = new Mock<IClockingReadOnlyRepository>();
            var moqClockingWriteOnlyRepository = new Mock<IClockingWriteOnlyRepository>();
            var moqTimeSheetShiftWriteOnlyRepository = new Mock<ITimeSheetShiftWriteOnlyRepository>();
            var moqEmployeeReadOnlyRepository = new Mock<IEmployeeReadOnlyRepository>();
            var moqTimeSheetReadOnlyRepository = new Mock<ITimeSheetReadOnlyRepository>();

            var moqGeneralTimeSheetValidator = new GeneralTimeSheetValidator(moqEmployeeReadOnlyRepository.Object,
                moqShiftReadOnlyRepository.Object, moqTimeSheetReadOnlyRepository.Object);

            moqEmployeeReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Employee>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Employee> findEmployeeSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Employees.Where(findEmployeeSpec.GetExpression()).ToList());

            moqShiftReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Shift>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Shift> findShiftSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Shifts.Where(findShiftSpec.GetExpression()).ToList());

            moqClockingWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Clocking>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<Clocking> findClockingSpec, string include) =>
                    InMemoryDb.Context.Clockings.Where(findClockingSpec.GetExpression()).ToList());

            moqClockingReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Clocking>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Clocking> findClockingSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Clockings.Where(findClockingSpec.GetExpression()).ToList());

            _generateClockingsDomainService = new GenerateClockingsDomainService(moqMapper.Object,
                moqBranchSettingReadOnlyRepository.Object, moqHolidayReadOnlyRepository.Object,
                moqShiftReadOnlyRepository.Object, moqClockingReadOnlyRepository.Object,
                moqClockingWriteOnlyRepository.Object, moqGeneralTimeSheetValidator,
                moqTimeSheetShiftWriteOnlyRepository.Object);
        }

        [Fact]
        public async Task Clockings_Should_Be_Generated_When_Do_Generate_Clocking_For_Time_Sheet()
        {
            // Arrange
            InMemoryDb.Context.TimeSheets.Each(x => x.BranchId = 2);
            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto
            {
                TimeSheets = new List<TimeSheet> { InMemoryDb.Context.TimeSheets.FirstOrDefault() },
                ApplyTimes = new List<DateRangeDto> { new DateRangeDto { From = DateTime.Now } },
                GenerateByType = GenerateClockingByType.TimeSheet
            };

            var expectedResultIsValid = true;

            // Act
            var result = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDto);
            var actResult = result.IsValid;

            // Assert
            Assert.Equal(expectedResultIsValid, actResult);
        }

        [Fact]
        public async Task Method_Should_Be_Showed_Employee_Validation_Error()
        {
            var timeSheet = InMemoryDb.Context.TimeSheets.FirstOrDefault();
            if (timeSheet != null) timeSheet.BranchId = 1;

            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto
            {
                TimeSheets = new List<TimeSheet> { timeSheet },
                ApplyTimes = new List<DateRangeDto> { new DateRangeDto { From = DateTime.Now } },
                GenerateByType = GenerateClockingByType.TimeSheet
            };

            var expectedMessageError = "Nhân viên không cùng chi nhánh với lịch làm việc";

            // Act
            var result = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDto);
            if (timeSheet != null) timeSheet.BranchId = 2;

            // Assert
            Assert.Contains(expectedMessageError, result.ValidationErrors.ToList());
        }

        [Fact]
        public async Task Method_Should_Be_Showed_Shift_Validation_Error()
        {
            var timeSheet = InMemoryDb.Context.TimeSheets.FirstOrDefault();
            if (timeSheet != null) timeSheet.BranchId = 1;

            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto
            {
                TimeSheets = new List<TimeSheet> { timeSheet },
                ApplyTimes = new List<DateRangeDto> { new DateRangeDto { From = DateTime.Now } },
                GenerateByType = GenerateClockingByType.TimeSheet
            };

            var expectedMessageError = "Ca làm việc không cùng chi nhánh với lịch làm việc";

            // Act
            var result = await _generateClockingsDomainService.GenerateClockingForTimeSheets(generateClockingForTimeSheetsDto);
            if (timeSheet != null) timeSheet.BranchId = 2;

            // Assert
            Assert.Contains(expectedMessageError, result.ValidationErrors.ToList());
        }
    }
}
