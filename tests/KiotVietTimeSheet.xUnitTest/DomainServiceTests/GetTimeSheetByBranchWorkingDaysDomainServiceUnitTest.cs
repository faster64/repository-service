using System;
using System.Collections.Generic;
using System.Linq;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class GetTimeSheetByBranchWorkingDaysDomainServiceUnitTest
    {
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();
        private readonly GetTimeSheetByBranchWorkingDaysDomainService _getTimeSheetByBranchWorkingDaysDomainService;

        public GetTimeSheetByBranchWorkingDaysDomainServiceUnitTest()
        {
            var moqTimeSheetWriteOnlyRepository = new Mock<ITimeSheetWriteOnlyRepository>();

            moqTimeSheetWriteOnlyRepository
                .Setup(x => x.GetTimeSheetForBranchSetting(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(() => InMemoryDb.Context.TimeSheets.ToList());

            _getTimeSheetByBranchWorkingDaysDomainService = new GetTimeSheetByBranchWorkingDaysDomainService(moqTimeSheetWriteOnlyRepository.Object);
        }

        [Fact]
        public void Get_Time_Sheet_By_Branch_Working_Day()
        {
            var branchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault();

            var getTimeSheetByBranchWorkingDaysDto = new GetTimeSheetByBranchWorkingDaysDto()
            {
                BranchId = branchId,
                ApplyFrom = DateTime.Now,
                WorkingDays = new List<byte>() { 1,2,3,4,5 }
            };

            // Act
            var listTimeSheet = _getTimeSheetByBranchWorkingDaysDomainService.GetTimeSheetByBranchWorkingDay(getTimeSheetByBranchWorkingDaysDto);

            // Assert
            Assert.NotNull(listTimeSheet);
        }

        [Fact]
        public void Cannot_Get_Time_Sheet_If_Branch_Working_Day_Have_Null()
        {
            var branchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault();

            var getTimeSheetByBranchWorkingDaysDto = new GetTimeSheetByBranchWorkingDaysDto()
            {
                BranchId = branchId,
                ApplyFrom = DateTime.Now,
                WorkingDays = new List<byte>()
            };

            List<TimeSheet> expectedValue = null;

            // Act
            var listTimeSheet = _getTimeSheetByBranchWorkingDaysDomainService.GetTimeSheetByBranchWorkingDay(getTimeSheetByBranchWorkingDaysDto);

            // Assert
            Assert.Equal(expectedValue, listTimeSheet);
        }
    }
}
