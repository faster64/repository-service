using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.Dtos;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PaysheetAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.PayslipAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Auth;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class DetectionChangePaysheetDomainServiceUnitTest
    {
        private readonly DetectionChangePaysheetDomainService _detectionChangePaysheetDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public DetectionChangePaysheetDomainServiceUnitTest()
        {
            var moqEmployeeReadOnlyRepository = new Mock<IEmployeeReadOnlyRepository>();
            var moqKiotVietServiceClient = new Mock<IKiotVietServiceClient>();
            var moqAuthService = new Mock<IAuthService>();
            var moqMapper = new Mock<IMapper>();
            var moqPaysheetReadOnlyRepository = new Mock<IPaysheetReadOnlyRepository>();
            var moqClockingReadOnlyRepository = new Mock<IClockingReadOnlyRepository>();

            moqPaysheetReadOnlyRepository
                .Setup(x => x.FindBySpecificationAsync(It.IsAny<ISpecification<Paysheet>>(), It.IsAny<bool>(),
                    It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Paysheet> findPaysheetSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Paysheet.FirstOrDefault());

            moqEmployeeReadOnlyRepository
                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Employee>>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((ISpecification<Employee> findEmployeeSpec, bool reference, bool includeSoftDelete) =>
                    InMemoryDb.Context.Employees.Where(findEmployeeSpec.GetExpression()).ToList());

            moqKiotVietServiceClient
                .Setup(x => x.GetUserByRevenue(It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<List<long>>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new List<UserByRevenueObject>());

            moqMapper.Setup(x => x.Map<List<PayslipDto>>(It.IsAny<List<Payslip>>()))
                .Returns(() => new List<PayslipDto>());

            moqClockingReadOnlyRepository
                .Setup(x => x.GetClockingForPaySheet(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 
                    It.IsAny<List<long>>()))
                .ReturnsAsync(() => InMemoryDb.Context.Clockings.ToList());

            moqAuthService.Setup(x => x.Context).Returns(() => new ExecutionContext()
            {
                BranchId = InMemoryDb.Context.BranchSettings.Select(x => x.BranchId).FirstOrDefault(),
                TenantId = 1
            });

            //_detectionChangePaysheetDomainService = new DetectionChangePaysheetDomainService(
            //    moqEmployeeReadOnlyRepository.Object, moqKiotVietServiceClient.Object, moqAuthService.Object,
            //    moqMapper.Object, moqPaysheetReadOnlyRepository.Object, moqClockingReadOnlyRepository.Object);
        }

        [Fact]
        public async Task Paysheet_Is_Changed_If_Have_Difference_Version()
        {
            var paysheet = new Paysheet
            {
                Version = 1
            };

            List<Payslip> payslips = new List<Payslip>();
            var expectedValue = true;

            // Act
            var isChangePaysheet = await _detectionChangePaysheetDomainService.IsChangePaysheetAsync(paysheet, payslips);

            // Assert
            Assert.Equal(expectedValue, isChangePaysheet);
        }

        [Fact]
        public async Task Paysheet_Is_Not_Changed()
        {
            var paysheet = InMemoryDb.Context.Paysheet.FirstOrDefault();
            var payslips = InMemoryDb.Context.Payslips.ToList();
            
            var expectedValue = false;

            // Act
            var isChangePaysheet = await _detectionChangePaysheetDomainService.IsChangePaysheetAsync(paysheet, payslips);

            // Assert
            Assert.Equal(expectedValue, isChangePaysheet);
        }

        [Fact]
        public async Task Paysheet_Is_Changed_If_Current_Version_Less_Than_Existing_Paysheet_Version()
        {
            var paysheet = InMemoryDb.Context.Paysheet.First();
            paysheet.Version = 2;

            var paysheetDto = new PaysheetDto()
            {
                Version = 0
            };

            var expectedValue = true;

            // Act
            var isChangePaysheet =
                await _detectionChangePaysheetDomainService.IsChangePaysheetWhenUpdatePaysheetAsync(paysheetDto);

            // Assert
            Assert.Equal(expectedValue, isChangePaysheet);
        }

        [Fact]
        public async Task Paysheet_Is_Not_Changed_When_Update_Paysheet()
        {
            var paysheet = InMemoryDb.Context.Paysheet.First();
            
            var paysheetDto = new PaysheetDto()
            {
                Id = paysheet.Id,
                Code = paysheet.Code,
                TenantId = paysheet.TenantId,
                BranchId = paysheet.BranchId,
                IsDeleted = paysheet.IsDeleted,
                DeletedBy = paysheet.DeletedBy,
                DeletedDate = paysheet.DeletedDate,
                CreatedBy = paysheet.CreatedBy,
                CreatedDate = paysheet.CreatedDate,
                Name = paysheet.Name,
                SalaryPeriod = paysheet.SalaryPeriod,
                StartTime = paysheet.StartTime,
                EndTime = paysheet.EndTime,
                PaysheetStatus = paysheet.PaysheetStatus,
                Note = paysheet.Note,
                Payslips = new List<PayslipDto>(),
                WorkingDayNumber = paysheet.WorkingDayNumber,
                PaysheetPeriodName = paysheet.PaysheetPeriodName,
                CreatorBy = paysheet.CreatorBy,
                PaysheetCreatedDate = paysheet.PaysheetCreatedDate,
                Version = paysheet.Version,
                IsDraft = paysheet.IsDraft
            };

            var expectedValue = false;

            // Act
            var isChangePaysheet =
                await _detectionChangePaysheetDomainService.IsChangePaysheetWhenUpdatePaysheetAsync(paysheetDto);

            // Assert
            Assert.Equal(expectedValue, isChangePaysheet);
        }

        [Fact]
        public async Task Paysheet_Is_Not_Changed_When_Change_Period()
        {
            var paysheet = InMemoryDb.Context.Paysheet.First();

            var paysheetDto = new PaysheetDto()
            {
                Id = paysheet.Id,
                Code = paysheet.Code,
                TenantId = paysheet.TenantId,
                BranchId = paysheet.BranchId,
                IsDeleted = paysheet.IsDeleted,
                DeletedBy = paysheet.DeletedBy,
                DeletedDate = paysheet.DeletedDate,
                CreatedBy = paysheet.CreatedBy,
                CreatedDate = paysheet.CreatedDate,
                Name = paysheet.Name,
                SalaryPeriod = paysheet.SalaryPeriod,
                StartTime = paysheet.StartTime.AddDays(-1),
                EndTime = paysheet.EndTime.AddDays(1),
                PaysheetStatus = paysheet.PaysheetStatus,
                Note = paysheet.Note,
                Payslips = new List<PayslipDto>(),
                WorkingDayNumber = paysheet.WorkingDayNumber,
                PaysheetPeriodName = paysheet.PaysheetPeriodName,
                CreatorBy = paysheet.CreatorBy,
                PaysheetCreatedDate = paysheet.PaysheetCreatedDate,
                Version = paysheet.Version,
                IsDraft = paysheet.IsDraft
            };

            var expectedValue = false;

            // Act
            var isChangePaysheet =
                await _detectionChangePaysheetDomainService.IsChangePaysheetWhenUpdatePaysheetAsync(paysheetDto);

            // Assert
            Assert.Equal(expectedValue, isChangePaysheet);
        }

        [Fact]
        public async Task Paysheet_Is_Changed_When_Make_Payment_With_Difference_Paysheet_Version()
        {
            // Arrange
            var paysheetId = InMemoryDb.Context.Paysheet.Select(x => x.Id).FirstOrDefault();
            var expextedValue = true;

            // Act 
            var isChangePaysheet =
                await _detectionChangePaysheetDomainService.IsChangePaysheetWhenMakePaymentsAsync(paysheetId, 1);

            // Assert
            Assert.Equal(expextedValue, isChangePaysheet);
        }

        [Fact]
        public async Task Paysheet_Is_Not_Changed_When_Make_Payment_With_Same_Paysheet_Version()
        {
            // Arrange
            var paysheet = InMemoryDb.Context.Paysheet.First();
            var expextedValue = false;

            // Act 
            var isChangePaysheet =
                await _detectionChangePaysheetDomainService.IsChangePaysheetWhenMakePaymentsAsync(paysheet.Id, paysheet.Version);

            // Assert
            Assert.Equal(expextedValue, isChangePaysheet);
        }
    }
}
