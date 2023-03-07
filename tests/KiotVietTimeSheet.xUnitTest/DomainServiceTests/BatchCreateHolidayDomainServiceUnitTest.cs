using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.DomainService.Impls;
using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.HolidayAggregate.Models;
using Moq;
using Xunit;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class BatchCreateHolidayDomainServiceUnitTest
    {
        private readonly BatchCreateHolidayDomainService _batchCreateHolidayDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public BatchCreateHolidayDomainServiceUnitTest()
        {
            var moqHolidayWriteOnlyRepository = new Mock<IHolidayWriteOnlyRepository>();
            var moqUpdateTenantNationalHolidayDomainService = new Mock<IUpdateTenantNationalHolidayDomainService>();

            moqHolidayWriteOnlyRepository
                .Setup(x => x.BatchAdd(It.IsAny<List<Holiday>>(), null))
                .Callback((List<Holiday> holidays) =>
                {
                    InMemoryDb.Context.Holidays.AddRange(holidays);
                    InMemoryDb.Context.SaveChanges();
                });

            moqHolidayWriteOnlyRepository
                .Setup(x => x.UnitOfWork.CommitAsync())
                .Returns(() => InMemoryDb.Context.SaveChangesAsync());

            _batchCreateHolidayDomainService = new BatchCreateHolidayDomainService(moqHolidayWriteOnlyRepository.Object,
                moqUpdateTenantNationalHolidayDomainService.Object);
        }

        [Fact]
        public async Task Holiday_Should_Be_Created_When_Do_Create()
        {
            List<Holiday> holidays = new List<Holiday>();

            for (int i = 0; i < 3; i++)
            {
                var holiday = new Holiday("Nghỉ lễ " + i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i));
                holidays.Add(holiday);
            }

            var expectedHolidayLength = holidays.Count + InMemoryDb.Context.Holidays.Count();

            // Act
            await _batchCreateHolidayDomainService.BatchCreateAutomaticAsync(holidays);

            var actHolidayLength = InMemoryDb.Context.Holidays.Count();

            //Assert
            Assert.Equal(expectedHolidayLength, actHolidayLength);
        }

        [Fact]
        public async Task Cannot_Create_Holiday_If_Holiday_Name_Have_Null()
        {
            List<Holiday> holidays = new List<Holiday>();

            for (int i = 0; i < 3; i++)
            {
                var holiday = new Holiday("", DateTime.Now.AddDays(i), DateTime.Now.AddDays(i));
                holidays.Add(holiday);
            }

            var expectedHolidayLength = InMemoryDb.Context.Holidays.Count();

            // Act
            await _batchCreateHolidayDomainService.BatchCreateAutomaticAsync(holidays);
            var actHolidayLength = InMemoryDb.Context.Holidays.Count();

            //Assert
            Assert.Equal(expectedHolidayLength, actHolidayLength);
        }

        [Fact]
        public async Task Cannot_Create_Holiday_If_Holiday_Name_Greater_Than_100_Character()
        {
            List<Holiday> holidays = new List<Holiday>();

            for (int i = 0; i < 3; i++)
            {
                var holiday =
                    new Holiday(
                        "2. Vào Bảng lương >> Click them mới bang lương kì hạn khác hang tháng (hang tuần/ 2 lần 1 tháng…) > hiển thị pop-up chọn đồng ý làm việc với bang lương nháp" + i,
                        DateTime.Now.AddDays(i), DateTime.Now.AddDays(i));
                holidays.Add(holiday);
            }

            var expectedHolidayLength = InMemoryDb.Context.Holidays.Count();

            // Act
            await _batchCreateHolidayDomainService.BatchCreateAutomaticAsync(holidays);
            var actHolidayLength = InMemoryDb.Context.Holidays.Count();

            //Assert
            Assert.Equal(expectedHolidayLength, actHolidayLength);
        }

        [Fact]
        public async Task Cannot_Create_Holiday_If_To_Greater_Than_From()
        {
            List<Holiday> holidays = new List<Holiday>();

            for (int i = 0; i < 3; i++)
            {
                var holiday = new Holiday("Nghỉ lễ " + i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(-(i+1)));
                holidays.Add(holiday);
            }

            var expectedHolidayLength = InMemoryDb.Context.Holidays.Count();

            // Act
            await _batchCreateHolidayDomainService.BatchCreateAutomaticAsync(holidays);

            var actHolidayLength = InMemoryDb.Context.Holidays.Count();

            //Assert
            Assert.Equal(expectedHolidayLength, actHolidayLength);
        }

        [Fact]
        public async Task Cannot_Create_Holiday_If_Total_Day_Greater_Than_31()
        {
            List<Holiday> holidays = new List<Holiday>();

            for (int i = 0; i < 3; i++)
            {
                var holiday = new Holiday("Nghỉ lễ " + i, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i + 31));
                holidays.Add(holiday);
            }

            var expectedHolidayLength = InMemoryDb.Context.Holidays.Count();

            // Act
            await _batchCreateHolidayDomainService.BatchCreateAutomaticAsync(holidays);

            var actHolidayLength = InMemoryDb.Context.Holidays.Count();

            //Assert
            Assert.Equal(expectedHolidayLength, actHolidayLength);
        }
    }
}
