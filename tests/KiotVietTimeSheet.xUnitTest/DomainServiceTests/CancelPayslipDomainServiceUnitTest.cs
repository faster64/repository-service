using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class CancelPayslipDomainServiceUnitTest
    {
        private readonly CancelPayslipDomainService _cancelPayslipDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public CancelPayslipDomainServiceUnitTest()
        {
            var moqEventDispatcher = new Mock<IEventDispatcher>();
            var moqPayslipWriteOnlyRepository = new Mock<IPayslipWriteOnlyRepository>();
            var moqPayslipClockingWriteOnlyRepository = new Mock<IPayslipClockingWriteOnlyRepository>();
            var moqClockingWriteOnlyRepository = new Mock<IClockingWriteOnlyRepository>();
            var moqPaySheetOutOfDateDomainService = new Mock<IPaySheetOutOfDateDomainService>();

            moqPayslipWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Payslip>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<Payslip> findPayslipSpec, string include) =>
                    InMemoryDb.Context.Payslips.Where(findPayslipSpec.GetExpression()).ToList());

            moqPayslipWriteOnlyRepository
                .Setup(x => x.UnitOfWork.CommitAsync())
                .Returns(() => InMemoryDb.Context.SaveChangesAsync());

            moqPayslipClockingWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<PayslipClocking>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<PayslipClocking> findPayslipClockingSpec, string include) =>
                    InMemoryDb.Context.PayslipClockings.Where(findPayslipClockingSpec.GetExpression()).ToList());

            moqClockingWriteOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Clocking>>(), It.IsAny<string>()))
                .ReturnsAsync((ISpecification<Clocking> findClockingSpec, string include) =>
                    InMemoryDb.Context.Clockings.Where(findClockingSpec.GetExpression()).ToList());

            _cancelPayslipDomainService = new CancelPayslipDomainService(moqEventDispatcher.Object,
                moqPayslipWriteOnlyRepository.Object, moqPayslipClockingWriteOnlyRepository.Object,
                moqClockingWriteOnlyRepository.Object, moqPaySheetOutOfDateDomainService.Object);
        }

        [Fact]
        public async Task Payslip_Must_Be_Cancelled_When_Do_Cancel()
        {
            var payslips = InMemoryDb.Context.Payslips.ToList();
            var payslipIds = payslips.Select(x => x.Id).ToList();

            // Act
            await _cancelPayslipDomainService.CancelAsync(payslipIds);

            // Assert
            var payslipStatuses = InMemoryDb.Context.Payslips.Select(x => x.PayslipStatus).ToList();
            Assert.Contains((byte)PayslipStatuses.Void, payslipStatuses);
        }

        [Fact]
        public async Task Clocking_Must_Be_Updated_Payment_Status_To_Unpaid_When_Do_Cancel()
        {
            var payslips = InMemoryDb.Context.Payslips.ToList();
            var payslipIds = payslips.Select(x => x.Id).ToList();

            var clockings = InMemoryDb.Context.Clockings;
            foreach (var clocking in clockings)
            {
                clocking.ClockingPaymentStatus = (byte) ClockingPaymentStatuses.Paid;
            }

            // Act
            await _cancelPayslipDomainService.CancelAsync(payslipIds);

            // Assert
            var clockingPaymentStatuses = InMemoryDb.Context.Clockings.Select(x => x.ClockingPaymentStatus).ToList();

            Assert.Contains((byte)ClockingPaymentStatuses.UnPaid, clockingPaymentStatuses);
        }

        [Fact]
        public async Task Payslip_Clocking_Must_Be_Removed_When_Do_Cancel()
        {
            var payslips = InMemoryDb.Context.Payslips.ToList();
            var payslipIds = payslips.Select(x => x.Id).ToList();

            var expectedPayslipClockingCount = 0;

            // Act
            await _cancelPayslipDomainService.CancelAsync(payslipIds);
            var actPayslipClockingCount = InMemoryDb.Context.PayslipClockings.Count();

            // Assert
            Assert.Equal(expectedPayslipClockingCount, actPayslipClockingCount);
        }
    }
}
