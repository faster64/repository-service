using AutoMapper;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using Moq;

namespace KiotVietTimeSheet.UnitTest.DomainServiceTests
{
    public class CreateTimeSheetClockingDomainServiceUnitTest
    {
        private readonly CreateTimeSheetClockingDomainService _createTimeSheetClockingDomainService;
        private static readonly InMemoryDb InMemoryDb = new InMemoryDb();

        public CreateTimeSheetClockingDomainServiceUnitTest()
        {
            var moqShiftReadOnlyRepository = new Mock<IShiftReadOnlyRepository>();
            var moqTimeSheetWriteOnlyRepository = new Mock<ITimeSheetWriteOnlyRepository>();
            var moqGenerateClockingsDomainService = new Mock<IGenerateClockingsDomainService>();
            var moqClockingWriteOnlyRepository = new Mock<IClockingWriteOnlyRepository>();
            var moqEmployeeReadOnlyRepository = new Mock<IEmployeeReadOnlyRepository>();
            var moqMapper = new Mock<IMapper>();

            _createTimeSheetClockingDomainService = new CreateTimeSheetClockingDomainService(
                moqShiftReadOnlyRepository.Object, moqTimeSheetWriteOnlyRepository.Object,
                moqGenerateClockingsDomainService.Object, moqClockingWriteOnlyRepository.Object,
                moqEmployeeReadOnlyRepository.Object, moqMapper.Object);
        }

//        [Fact]
//        public async Task Time_Sheet_Should_Be_Created_When_Do_Create()
//        {
//            TimeSheetDto timeSheetDto = new TimeSheetDto()
//            {
//                EmployeeId = 1,
//                StartDate = DateTime.Now,
//                IsRepeat = false,
//                RepeatType = 1,
//                RepeatEachDay = 1,
//                EndDate = DateTime.Now,
//                BranchId = 2,
//                SaveOnDaysOffOfBranch = false,
//                SaveOnHoliday = false,
//                TimeSheetShifts = new List<TimeSheetShiftDto>(),
//                Note = "New TimeSheet"
//            };
//
//            await _createTimeSheetClockingDomainService.CreateAsync(timeSheetDto);
//        }
    }
}
