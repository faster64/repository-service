using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class RejectClockingsDomainServiceUnitTest
    {
        private readonly RejectClockingsDomainService _rejectClockingsDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public RejectClockingsDomainServiceUnitTest()
        {
            var moqEventDispatcher = new Mock<IEventDispatcher>();
            var moqTimeSheetReadOnlyRepository = new Mock<ITimeSheetReadOnlyRepository>();
            var moqClockingHistoryWriteOnlyRepository = new Mock<IClockingHistoryWriteOnlyRepository>();
            var moqClockingWriteOnlyRepository = new Mock<IClockingWriteOnlyRepository>();
            var moqTimeSheetWriteOnlyRepository = new Mock<ITimeSheetWriteOnlyRepository>();

            moqTimeSheetWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<TimeSheet>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<TimeSheet> findTimeSheetSpec, string include) =>
                    InMemoryDb.Context.TimeSheets.Where(findTimeSheetSpec.GetExpression()).ToList());

            moqClockingWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Clocking>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<Clocking> findTimeSheetClockingSpec, string include) =>
                    InMemoryDb.Context.Clockings.Where(findTimeSheetClockingSpec.GetExpression()).ToList());

            moqClockingWriteOnlyRepository
                .Setup(x => x.BatchUpdate(It.IsAny<List<Clocking>>(), null))
                .Callback((List<Clocking> clockings) =>
                {
                    InMemoryDb.Context.Clockings.UpdateRange(clockings);
                    InMemoryDb.Context.SaveChanges();
                });

            moqTimeSheetWriteOnlyRepository
                .Setup(x => x.BatchUpdate(It.IsAny<List<TimeSheet>>(), null))
                .Callback((List<TimeSheet> timeSheets) =>
                {
                    InMemoryDb.Context.TimeSheets.UpdateRange(timeSheets);
                    InMemoryDb.Context.SaveChanges();
                });

            _rejectClockingsDomainService = new RejectClockingsDomainService(moqEventDispatcher.Object,
                moqTimeSheetReadOnlyRepository.Object, moqClockingHistoryWriteOnlyRepository.Object,
                moqClockingWriteOnlyRepository.Object, moqTimeSheetWriteOnlyRepository.Object);
        }

        [Fact]
        public void Clocking_Should_Be_Rejected_When_Do_Reject()
        {
            var clockings = InMemoryDb.Context.Clockings.ToList();

            // Act
            var result = _rejectClockingsDomainService.RejectClockingsAsync(clockings);

            // Assert
            Assert.True(result.Result);
        }

        [Fact]
        public async Task Clocking_Should_Be_Updated_Status_When_Do_Reject()
        {
            var clockings = InMemoryDb.Context.Clockings.ToList();

            // Act
            await _rejectClockingsDomainService.RejectClockingsAsync(clockings);

            var clockingStatuses = InMemoryDb.Context.Clockings.Select(x => x.ClockingStatus).ToList();

            // Assert
            Assert.Contains((byte)ClockingStatuses.Void, clockingStatuses);
        }

        [Fact]
        public async Task Time_Sheet_Should_Be_Updated_Status_When_Do_Reject()
        {
            var clockings = InMemoryDb.Context.Clockings.ToList();

            // Act
            await _rejectClockingsDomainService.RejectClockingsAsync(clockings);

            var timeSheetStatuses = InMemoryDb.Context.TimeSheets.Select(x => x.TimeSheetStatus).ToList();

            // Assert
            Assert.Contains((byte)TimeSheetStatuses.Void, timeSheetStatuses);
        }

        [Fact]
        public async Task Clocking_Cannot_Be_Reject_If_Total_Clocking_Greater_Than_3000()
        {
            var clockings = new List<Clocking>();
            for (int i = 6; i <= 4000; i++)
            {
                clockings.Add(new Clocking(i, 1, 1, 0 + i, 1, 1, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i),
                    "", 1, 2, 1, DateTime.Now,
                    null, null, false, null, null, null, null, 0, 0, 0, 0, null, new List<ClockingHistory>(), 0,
                    new Employee(), new Employee(), new TimeSheet()));
            }
            InMemoryDb.Context.Clockings.AddRange(clockings);

            // Act
            var result = await _rejectClockingsDomainService.RejectClockingsAsync(clockings);

            InMemoryDb.Context.Clockings.RemoveRange(clockings);

            // Assert
            Assert.False(result);
        }
    }
}
