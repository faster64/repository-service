using System;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class WorkingDayForPaysheetDomainServiceUnitTest
    {
        private const int BranchId = 2;
        private readonly WorkingDayForPaysheetDomainService _workingDayForPaysheetDomainService;

        public WorkingDayForPaysheetDomainServiceUnitTest()
        {
            var moqBrachSettingReadOnlyRepository = new Mock<IBranchSettingReadOnlyRepository>();
            _workingDayForPaysheetDomainService = new WorkingDayForPaysheetDomainService(moqBrachSettingReadOnlyRepository.Object);
        }

        [Fact]
        public async Task Get_Working_Day_Number_For_Paysheet()
        {
            var from = DateTime.Now;
            var to = DateTime.Now.AddDays(1);
            var expectedValue = 2;

            // Act
            var workingDays = await _workingDayForPaysheetDomainService.GetWorkingDayPaysheetAsync(BranchId, from, to);
            var workingDaysNumber = int.Parse(workingDays.ToString());

            // Assert
            Assert.Equal(expectedValue, workingDaysNumber);
        }

        [Fact]
        public async Task Working_Day_Number_Not_Equal_Expected()
        {
            var from = DateTime.Now;
            var to = DateTime.Now.AddDays(1);
            var expectedValue = 5;

            // Act
            var workingDays = await _workingDayForPaysheetDomainService.GetWorkingDayPaysheetAsync(BranchId, from, to);
            var workingDaysNumber = int.Parse(workingDays.ToString());

            // Assert
            Assert.NotEqual(expectedValue, workingDaysNumber);
        }
    }
}
