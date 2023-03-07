using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class CompleteSalaryClockingDomainServiceUnitTest
    {
        private readonly CompleteSalaryClockingDomainService _completeSalaryClockingDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public CompleteSalaryClockingDomainServiceUnitTest()
        {
            var clockingsDb = InMemoryDb.Context.Clockings;
            var moqClockingWriteOnlyRepository = new Mock<IClockingWriteOnlyRepository>();
            var moqPaySheetOutOfDateDomainService = new Mock<IPaySheetOutOfDateDomainService>();
            var moqPayslipClockingWriteOnlyRepository = new Mock<IPayslipClockingWriteOnlyRepository>();

            moqClockingWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<FindClockingByIdsSpec>(), It.IsAny<string>()))
                .ReturnsAsync((FindClockingByIdsSpec findClockingByIdsSpec, string include) =>
                    clockingsDb
                        .Where(findClockingByIdsSpec.GetExpression()).ToList());

            moqClockingWriteOnlyRepository
                .Setup(x => x.BatchUpdate(It.IsAny<List<Clocking>>(), null))
                .Callback((List<Clocking> clockings) =>
                {
                    InMemoryDb.Context.Clockings.UpdateRange(clockings);
                    InMemoryDb.Context.SaveChanges();
                });

            _completeSalaryClockingDomainService = new CompleteSalaryClockingDomainService(
                moqClockingWriteOnlyRepository.Object, moqPaySheetOutOfDateDomainService.Object,
                moqPayslipClockingWriteOnlyRepository.Object);
        }

        [Fact]
        public async Task Clocking_Must_Be_Update_Payment_Status_When_Complete_Paysheet()
        {
            var paysheet = InMemoryDb.Context.Paysheet.Include("Payslips").FirstOrDefault(x => x.BranchId == 2);
            var payslipClockings = InMemoryDb.Context.PayslipClockings
                .Where(x => paysheet.Payslips.Select(p => p.Id).Contains(x.PayslipId)).ToList();

            if (paysheet != null)
            {
                foreach (var payslip in paysheet.Payslips)
                {
                    payslip.PayslipClockings = payslipClockings.Where(x => x.PayslipId == payslip.Id).ToList();
                }

                // Act
                await _completeSalaryClockingDomainService.CompletePaysheetForClockingsAsync(paysheet);
            }

            var clockingPaymentStatuses = InMemoryDb.Context.Clockings.Select(x => x.ClockingPaymentStatus).ToList();

            // Assert
            Assert.Contains((byte) ClockingPaymentStatuses.Paid, clockingPaymentStatuses);
        }
    }
}
