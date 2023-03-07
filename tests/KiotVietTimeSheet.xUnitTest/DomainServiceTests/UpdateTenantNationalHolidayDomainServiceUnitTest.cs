using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class UpdateTenantNationalHolidayDomainServiceUnitTest
    {
        private readonly UpdateTenantNationalHolidayDomainService _updateTenantNationalHolidayDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public UpdateTenantNationalHolidayDomainServiceUnitTest()
        {
            var moqTenantNationalHolidayWriteOnlyRepository = new Mock<ITenantNationalHolidayWriteOnlyRepository>();

            moqTenantNationalHolidayWriteOnlyRepository
                .Setup(x => x.FindBySpecification(It.IsAny<ISpecification<TenantNationalHoliday>>()))
                .Returns((ISpecification<TenantNationalHoliday> findTenantNationalHolidaySpec) =>
                    InMemoryDb.Context.TenantNationalHolidays.FirstOrDefault());

            moqTenantNationalHolidayWriteOnlyRepository
                .Setup(x => x.FindBySpecificationAsync(It.IsAny<ISpecification<TenantNationalHoliday>>(), It.IsAny<string>()))
                .ReturnsAsync(() => InMemoryDb.Context.TenantNationalHolidays.FirstOrDefault());

            moqTenantNationalHolidayWriteOnlyRepository
                .Setup(x => x.Add(It.IsAny<TenantNationalHoliday>(), null))
                .Callback((TenantNationalHoliday tenantNationalHoliday) =>
                {
                    InMemoryDb.Context.TenantNationalHolidays.Add(tenantNationalHoliday);
                    InMemoryDb.Context.SaveChanges();
                });

            moqTenantNationalHolidayWriteOnlyRepository
                .Setup(x => x.Update(It.IsAny<TenantNationalHoliday>(), null))
                .Callback((TenantNationalHoliday tenantNationalHoliday) =>
                {
                    InMemoryDb.Context.TenantNationalHolidays.UpdateRange(tenantNationalHoliday);
                    InMemoryDb.Context.SaveChanges();
                });

            _updateTenantNationalHolidayDomainService =
                new UpdateTenantNationalHolidayDomainService(moqTenantNationalHolidayWriteOnlyRepository.Object);
        }

        [Fact]
        public async Task Tenant_National_Holiday_Should_Be_Created_When_Do_Create()
        {
            var year = 2019;

            var expectedYear = 2019;

            // Act
            await _updateTenantNationalHolidayDomainService.UpdateTenantNationalHolidayAsync(year);
            var actYear = InMemoryDb.Context.TenantNationalHolidays.Select(x => x.LastCreatedYear).FirstOrDefault();

            // Assert
            Assert.Equal(expectedYear, actYear);
        }

        [Fact]
        public async Task Tenant_National_Holiday_Should_Be_Updated_When_Do_Update()
        {
            InMemoryDb.Context.TenantNationalHolidays.AddRange(new TenantNationalHoliday(2019));
            InMemoryDb.Context.SaveChanges();

            var expectedYear = 2020;

            // Act
            await _updateTenantNationalHolidayDomainService.UpdateTenantNationalHolidayAsync(expectedYear);
            var actYear = InMemoryDb.Context.TenantNationalHolidays.Select(x => x.LastCreatedYear).FirstOrDefault();

            // Assert
            Assert.Equal(expectedYear, actYear);
        }
    }
}
