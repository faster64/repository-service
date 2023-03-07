using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class PaySheetOutOfDateDomainServiceUnitTest
    {
        private readonly PaySheetOutOfDateDomainService _paySheetOutOfDateDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public PaySheetOutOfDateDomainServiceUnitTest()
        {
            var moqPaysheetWriteOnlyRepository = new Mock<IPaysheetWriteOnlyRepository>();

            moqPaysheetWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Paysheet>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<Paysheet> findPaysheetSpec, string include) =>
                    InMemoryDb.Context.Paysheet.Where(findPaysheetSpec.GetExpression()).ToList());

            moqPaysheetWriteOnlyRepository
                .Setup(x => x.GetPaysheetDraftAndTempByEmployeeIds(It.IsAny<List<long>>()))
                .ReturnsAsync(() => InMemoryDb.Context.Paysheet.ToList());

            moqPaysheetWriteOnlyRepository
                .Setup(x => x.GetPaysheetDraftAndTempByClockings(It.IsAny<List<Clocking>>()))
                .ReturnsAsync(() => InMemoryDb.Context.Paysheet.ToList());

            moqPaysheetWriteOnlyRepository
                .Setup(x => x.BatchUpdate(It.IsAny<List<Paysheet>>(), null))
                .Callback((List<Paysheet> paysheets) =>
                {
                  
                    InMemoryDb.Context.SaveChanges();
                });

            _paySheetOutOfDateDomainService = new PaySheetOutOfDateDomainService(moqPaysheetWriteOnlyRepository.Object);
        }

        [Fact]
        public async Task Paysheet_Should_Be_Updated_When_Do_Update_Paysheet()
        {
            var paysheetIds = InMemoryDb.Context.Paysheet.Select(x => x.Id).ToList();
            var versionOfPaysheet = 0;

            // Act
            await _paySheetOutOfDateDomainService.WithPaysheetChangeAsync(paysheetIds);

            // Assert
            var paysheetVersions = InMemoryDb.Context.Paysheet.Select(x => x.Version).ToList();
            Assert.DoesNotContain(versionOfPaysheet, paysheetVersions);
        }

        [Fact]
        public async Task Paysheet_Should_Be_Updated_When_Do_Update_Payrate()
        {
            var employeeIds = InMemoryDb.Context.Employees.Select(x => x.Id).ToList();
            var versionOfPaysheet = 0;

            // Act
            await _paySheetOutOfDateDomainService.WithPayRateDataChangeAsync(employeeIds);

            // Assert
            var paysheetVersions = InMemoryDb.Context.Paysheet.Select(x => x.Version).ToList();
            Assert.DoesNotContain(versionOfPaysheet, paysheetVersions);
        }

        [Fact]
        public async Task Paysheet_Should_Be_Updated_When_Do_Update_Clocking()
        {
            var clockings = InMemoryDb.Context.Clockings.ToList();
            var versionOfPaysheet = 0;

            // Act
            await _paySheetOutOfDateDomainService.WithClockingDataChangeAsync(clockings);

            // Assert
            var paysheetVersions = InMemoryDb.Context.Paysheet.Select(x => x.Version).ToList();
            Assert.DoesNotContain(versionOfPaysheet, paysheetVersions);
        }

        [Fact]
        public async Task Paysheet_Should_Be_Updated_When_Do_Update_Setting()
        {
            var branchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault();
            var versionOfPaysheet = 0;

            // Act
            await _paySheetOutOfDateDomainService.WithSettingsChange(branchId);

            // Assert
            var paysheetVersions = InMemoryDb.Context.Paysheet.Select(x => x.Version).ToList();
            Assert.DoesNotContain(versionOfPaysheet, paysheetVersions);
        }

        [Fact]
        public async Task Paysheet_Should_Be_Updated_When_Do_Update_Holiday()
        {
            var versionOfPaysheet = 0;

            // Act
            await _paySheetOutOfDateDomainService.WithHolidayChangeAsync(DateTime.Now, DateTime.Now);

            // Assert
            var paysheetVersions = InMemoryDb.Context.Paysheet.Select(x => x.Version).ToList();
            Assert.DoesNotContain(versionOfPaysheet, paysheetVersions);
        }

        [Fact]
        public async Task Paysheet_Should_Be_Updated_When_Do_Update_Branch_Setting()
        {
            var branchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault();
            var versionOfPaysheet = 0;

            // Act
            await _paySheetOutOfDateDomainService.WithChangeBranchSettingAsync(branchId);

            // Assert
            var paysheetVersions = InMemoryDb.Context.Paysheet.Select(x => x.Version).ToList();
            Assert.DoesNotContain(versionOfPaysheet, paysheetVersions);
        }
    }
}
