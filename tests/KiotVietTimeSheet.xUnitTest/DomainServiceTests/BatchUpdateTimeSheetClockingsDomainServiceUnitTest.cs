using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.DomainService.Enums;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class BatchUpdateTimeSheetClockingsDomainServiceUnitTest
    {
        private readonly BatchUpdateTimeSheetClockingsDomainService _batchUpdateTimeSheetClockingsDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public BatchUpdateTimeSheetClockingsDomainServiceUnitTest()
        {
            var moqGenerateClockingsDomainService = new Mock<IGenerateClockingsDomainService>();
            var moqTimeSheetWriteOnlyRepository = new Mock<ITimeSheetWriteOnlyRepository>();
            var moqTimeSheetShiftWriteOnlyRepository = new Mock<ITimeSheetShiftWriteOnlyRepository>();
            var moqClockingWriteOnlyRepository = new Mock<IClockingWriteOnlyRepository>();
            var moqMapper = new Mock<IMapper>();
            var moqTimeSheetIntegrationEventService = new Mock<ITimeSheetIntegrationEventService>();

            moqGenerateClockingsDomainService
                .Setup(x => x.GenerateClockingForTimeSheets(It.IsAny<GenerateClockingForTimeSheetsDto>()))
                .ReturnsAsync(() => new GenerateClockingsDomainServiceDto()
                {
                    TimeSheets = InMemoryDb.Context.TimeSheets.ToList(),
                    IsValid = true,
                    TimeSheetClockings = InMemoryDb.Context.Clockings.ToList(),
                    ClockingNeedUpdateIds = InMemoryDb.Context.Clockings.Select(x => x.Id).ToList()
                });

            moqTimeSheetWriteOnlyRepository
                .Setup(x => x.BatchDetachByClone(It.IsAny<List<TimeSheet>>(), It.IsAny<string[]>()))
                .Returns(() => InMemoryDb.Context.TimeSheets.ToList());

            moqTimeSheetShiftWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<TimeSheetShift>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<TimeSheetShift> findTimeSheetShiftSpec, string include) =>
                    InMemoryDb.Context.TimeSheetShifts.Where(findTimeSheetShiftSpec.GetExpression()).ToList());

            _batchUpdateTimeSheetClockingsDomainService = new BatchUpdateTimeSheetClockingsDomainService(
                moqGenerateClockingsDomainService.Object, moqTimeSheetWriteOnlyRepository.Object,
                moqTimeSheetShiftWriteOnlyRepository.Object, moqClockingWriteOnlyRepository.Object,
                moqMapper.Object, moqTimeSheetIntegrationEventService.Object);
        }

        [Fact]
        public async Task Cannot_Update_Clocking_When_Update_DaysOff_If_Time_Sheet_Have_Null()
        {
            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto();
            var errMessage = "Lịch làm việc đã bị hủy. Vui lòng kiểm tra lại";

            // Act
            var result = await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(
                generateClockingForTimeSheetsDto);

            // Assert
            Assert.Contains(errMessage, result.ValidationErrors);
        }

        [Fact]
        public async Task Clocking_Should_Be_Update_Clocking_When_Update_DaysOff()
        {
            var generateClockingForTimeSheetsDto = new GenerateClockingForTimeSheetsDto()
            {
                TimeSheets = InMemoryDb.Context.TimeSheets.ToList(),
                GenerateByType = GenerateClockingByType.TimeSheet

            };

            var expectedResultIsValid = true;

            // Act
            var result = await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetClockingWhenUpdateDaysOffAsync(
                generateClockingForTimeSheetsDto);

            var actResult = result.IsValid;

            // Assert
            Assert.Equal(expectedResultIsValid, actResult);
        }

        [Fact]
        public async Task Time_Sheet_And_Clocking_Should_Be_Updated_When_Change_DaysOff()
        {
            var timeSheets = InMemoryDb.Context.TimeSheets.ToList();
            var clockings = InMemoryDb.Context.Clockings.ToList();
            var clockingNeedUpdateIds = InMemoryDb.Context.Clockings.Select(x => x.Id).ToList();

            var expectedTimeSheetLength = timeSheets.Count;
            var expectedClockingLength = clockings.Count;

            // Act
            await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetAndClockingWhenChangeDayOffAsync(
                timeSheets, clockings, clockingNeedUpdateIds);

            var actTimeSheetLength = InMemoryDb.Context.TimeSheets.Count();
            var actClockingLength = InMemoryDb.Context.Clockings.Count();

            // Assert
            Assert.Equal(expectedTimeSheetLength, actTimeSheetLength);
            Assert.Equal(expectedClockingLength, actClockingLength);
        }

        [Fact]
        public async Task Time_Sheet_And_Clocking_Should_Be_Updated()
        {
            var timeSheets = InMemoryDb.Context.TimeSheets.ToList();
            var clockings = InMemoryDb.Context.Clockings.ToList();
            var clockingNeedUpdateIds = InMemoryDb.Context.Clockings.Select(x => x.Id).ToList();

            var expectedTimeSheetLength = timeSheets.Count;
            var expectedClockingLength = clockings.Count;
            var expectedTimeSheetShiftId = 0;

            // Act
            await _batchUpdateTimeSheetClockingsDomainService.BatchUpdateTimeSheetAndClocking(
                timeSheets, clockings, clockingNeedUpdateIds, (null, null, false));

            var actTimeSheetLength = InMemoryDb.Context.TimeSheets.Count();
            var actClockingLength = InMemoryDb.Context.Clockings.Count();
            var actTimeSheetShiftId = InMemoryDb.Context.TimeSheetShifts.FirstOrDefault()?.Id;

            // Assert
            Assert.Equal(expectedTimeSheetLength, actTimeSheetLength);
            Assert.Equal(expectedClockingLength, actClockingLength);
            Assert.Equal(expectedTimeSheetShiftId, actTimeSheetShiftId);
        }
    }
}
