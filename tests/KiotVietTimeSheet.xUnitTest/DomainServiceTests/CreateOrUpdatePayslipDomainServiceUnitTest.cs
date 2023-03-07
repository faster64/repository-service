using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class CreateOrUpdatePayslipDomainServiceUnitTest
    {
        private readonly CreateOrUpdatePayslipDomainService _createOrUpdatePayslipDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public CreateOrUpdatePayslipDomainServiceUnitTest()
        {
            var moqPayslipWriteOnlyRepository = new Mock<IPayslipWriteOnlyRepository>();
            var moqPayslipClockingWriteOnlyRepository = new Mock<IPayslipClockingWriteOnlyRepository>();
            var moqAuthService = new Mock<IAuthService>();
            var moqPayslipReadOnlyRepository = new Mock<IPayslipReadOnlyRepository>();
            var moqEmployeeReadOnlyRepository = new Mock<IEmployeeReadOnlyRepository>();

            moqPayslipReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Payslip>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Payslip> findPayslipSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Payslips.Where(findPayslipSpec.GetExpression()).ToList());

            moqEmployeeReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Employee>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Employee> findEmployeeSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Employees.Where(findEmployeeSpec.GetExpression()).ToList());

            moqPayslipReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Payslip>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Payslip> findPayslipSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Payslips.Where(findPayslipSpec.GetExpression()).ToList());

            moqAuthService.Setup(x => x.Context).Returns(() => new ExecutionContext()
            {
                BranchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault()
            });

            moqPayslipWriteOnlyRepository
                .Setup(x => x.BatchAdd(It.IsAny<List<Payslip>>(), null))
                .Callback((List<Payslip> payslips) =>
                {
                    InMemoryDb.Context.Payslips.AddRange(payslips);
                    InMemoryDb.Context.SaveChanges();
                });

            _createOrUpdatePayslipDomainService = new CreateOrUpdatePayslipDomainService(
                moqPayslipWriteOnlyRepository.Object, moqPayslipClockingWriteOnlyRepository.Object,
                moqAuthService.Object, moqPayslipReadOnlyRepository.Object, moqEmployeeReadOnlyRepository.Object);
        }

        [Fact]
        public async Task Payslips_Should_Be_Created_When_Do_Create()
        {
            var payslips = new List<Payslip>();
            for (int i = 6; i <= 7; i++)
            {
                payslips.Add(new Payslip(0 + i, "NewPL" + i, 1, 1, false, null, null, "", 1, 1, DateTime.Now, 1,
                    null, null, 0, 0, 0, 0, 0, 0, 0));
            }

            var expectedPayslipLength = InMemoryDb.Context.Payslips.Count() + payslips.Count;

            // Act
            await _createOrUpdatePayslipDomainService.BatchCreateAsync(payslips, DateTime.Now, DateTime.Now);

            // Assert
            Assert.Equal(expectedPayslipLength, InMemoryDb.Context.Payslips.Count());
        }

        [Fact]
        public async Task Payslips_Should_Be_Showed_Error_Message_If_Have_Same_Code()
        {
            var payslips = new List<Payslip>();
            for (int i = 6; i <= 7; i++)
            {
                payslips.Add(new Payslip(0 + i, "NewPL", 1, 1, false, null, null, "", 1, 1, DateTime.Now, 1,
                    null, null, 0, 0, 0, 0, 0, 0, 0));
            }

            var employeeName = InMemoryDb.Context.Employees.Where(x => x.Id == payslips.FirstOrDefault().EmployeeId)
                .Select(x => x.Name).ToList().FirstOrDefault();

            var expectedErrorMessage = "Nhân viên " + employeeName + " đang trùng mã phiếu lương";

            // Act
            var result = await _createOrUpdatePayslipDomainService.BatchCreateAsync(payslips, DateTime.Now, DateTime.Now);

            // Assert
            Assert.Contains(expectedErrorMessage, result.ValidationErrors);
        }

//        [Fact]
//        public async Task Payslips_Should_Be_Showed_Error_Message_If_Have_Existed_Code()
//        {
//            var payslips = new List<Payslip>();
//            for (int i = 5; i <= 6; i++)
//            {
//                payslips.Add(new Payslip(1 + i, "PL00" + i, 1, 1, false, null, null, "", 1, 1, DateTime.Now, 1,
//                    null, null, 0, 0, 0, 0, 0, 0, 0));
//            }
//
//            var employeeName = InMemoryDb.Context.Employees.Where(x => x.Id == payslips.FirstOrDefault().EmployeeId)
//                .Select(x => x.Name).ToList().FirstOrDefault();
//
//            var payslipCode = payslips.FirstOrDefault()?.Code;
//
//            var expectedErrorMessage = "Mã phiếu lương " + payslipCode + " của nhân viên " + employeeName + " đã tồn tại trong hệ thống.";
//
//            // Act
//            var result = await _createOrUpdatePayslipDomainService.BatchCreateAsync(payslips, DateTime.Now, DateTime.Now);
//
//            // Assert
//            Assert.Contains(expectedErrorMessage, result.ValidationErrors);
//        }
    }
}
