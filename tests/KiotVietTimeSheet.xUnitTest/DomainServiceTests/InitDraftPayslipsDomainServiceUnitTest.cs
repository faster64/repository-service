using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.AggregatesModels.AllowanceAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.DeductionAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayRateAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class InitDraftPayslipsDomainServiceUnitTest
    {
        private readonly InitDraftPayslipsDomainService _initDraftPayslipsDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public InitDraftPayslipsDomainServiceUnitTest()
        {
            var moqHolidayReadOnlyRepository = new Mock<IHolidayReadOnlyRepository>();
            var moqBranchSettingReadOnlyRepository = new Mock<IBranchSettingReadOnlyRepository>();
            var moqEmployeeReadOnlyRepository = new Mock<IEmployeeReadOnlyRepository>();
            var moqClockingReadOnlyRepository = new Mock<IClockingReadOnlyRepository>();
            var moqPayRateReadOnlyRepository = new Mock<IPayRateReadOnlyRepository>();
            var moqDeductionReadOnlyRepository = new Mock<IDeductionReadOnlyRepository>();
            var moqAllowanceReadOnlyRepository = new Mock<IAllowanceReadOnlyRepository>();
            var moqCommissionReadOnlyRepository = new Mock<ICommissionReadOnlyRepository>();
            var moqShiftReadOnlyRepository = new Mock<IShiftReadOnlyRepository>();
            var moqAuthService = new Mock<IAuthService>();
            var moqKiotVietServiceClient = new Mock<IKiotVietServiceClient>();
       

            moqEmployeeReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Employee>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Employee> findEmployeeSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Employees.Where(findEmployeeSpec.GetExpression()).ToList());

            moqClockingReadOnlyRepository
                .Setup(x => x.GetClockingForPaySheet(It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                     It.IsAny<List<long>>()))
                .ReturnsAsync(() => InMemoryDb.Context.Clockings.ToList());

            moqHolidayReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Holiday>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Holiday> findHolidaySpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Holidays.Where(findHolidaySpec.GetExpression()).ToList());

            moqShiftReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Shift>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Shift> findShiftSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Shifts.Where(findShiftSpec.GetExpression()).ToList());

            moqBranchSettingReadOnlyRepository
                .Setup(x => x.FindBranchSettingWithDefault(It.IsAny<ISpecification<BranchSetting>>()))
                .ReturnsAsync(() => InMemoryDb.Context.BranchSettings.FirstOrDefault());

            moqPayRateReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<PayRate>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<PayRate> findPayRateSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.PayRate.Where(findPayRateSpec.GetExpression()).ToList());

            moqDeductionReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Deduction>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Deduction> findDeductionSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Deduction.Where(findDeductionSpec.GetExpression()).ToList());

            moqAllowanceReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Allowance>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Allowance> findAllowanceSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Allowance.Where(findAllowanceSpec.GetExpression()).ToList());

            moqKiotVietServiceClient
                .Setup(x => x.GetUserByRevenue(It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<List<long>>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new List<UserByRevenueObject>());

            moqAuthService.Setup(x => x.Context).Returns(() => new ExecutionContext()
            {
                BranchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault(),
                TenantId = 1
            });

            _initDraftPayslipsDomainService = new InitDraftPayslipsDomainService(
                moqHolidayReadOnlyRepository.Object,
                moqBranchSettingReadOnlyRepository.Object, 
                moqEmployeeReadOnlyRepository.Object,
                moqClockingReadOnlyRepository.Object, 
                moqPayRateReadOnlyRepository.Object,
                moqDeductionReadOnlyRepository.Object, 
                moqAllowanceReadOnlyRepository.Object,
                moqShiftReadOnlyRepository.Object, 
                moqAuthService.Object, 
                moqKiotVietServiceClient.Object,
                moqCommissionReadOnlyRepository.Object
                );
        }

        [Fact]
        public async Task Payslip_Should_Be_Initted_When_Do_Init()
        {
            // Arrange
            var branchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault();
            var payslips = InMemoryDb.Context.Payslips.ToList();
            var employeeIds = InMemoryDb.Context.Employees.Select(x => x.Id).ToList();

            // Act
            var draftPayslips = await _initDraftPayslipsDomainService.InitDraftPayslipsAsync(payslips, employeeIds, DateTime.Now, DateTime.Now, 1,
                22, 8, branchId);

            // Assert
            Assert.NotNull(draftPayslips);
        }

        /// <summary>
        ///  /Kiểm tra trường hợp nếu không có điều kiện tính lương nào khác ngoài giảm trừ hoặc phụ cấp
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Payslip_Should_Be_Initted_If_Have_Only_Allowance_Or_Deduction_Rule()
        {
            // Arrange
            var branchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault();
            var payslips = InMemoryDb.Context.Payslips.ToList();
            var employeeIds = InMemoryDb.Context.Employees.Select(x => x.Id).ToList();

            InMemoryDb.Context.PayRateDetail.RemoveRange(InMemoryDb.Context.PayRateDetail.ToList());
            InMemoryDb.Context.SaveChanges();

            var payRateDetails = new List<PayRateDetail>
            {
                new PayRateDetail(1, "AllowanceRule",
                    "{\"AllowanceRuleValueDetails\":[{\"AllowanceId\":1,\"Name\":null,\"Value\":1000.0,\"ValueRatio\":null,\"Rank\":0}]}",
                    1),
                new PayRateDetail(1, "DeductionRule",
                    "{\"DeductionRuleValueDetails\":[{\"DeductionId\":1,\"Name\":null,\"Value\":null,\"ValueRatio\":2.0,\"Rank\":0}]}",
                    1)
            };

            InMemoryDb.Context.PayRateDetail.AddRange(payRateDetails);

            // Act
            var draftPayslips = await _initDraftPayslipsDomainService.InitDraftPayslipsAsync(payslips, employeeIds, DateTime.Now, DateTime.Now, 1,
                22, 8, branchId);

            InMemoryDb.Context.PayRateDetail.RemoveRange(payRateDetails);

            // Assert
            Assert.NotNull(draftPayslips);
        }
}
}
