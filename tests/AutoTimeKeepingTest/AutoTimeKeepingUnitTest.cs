//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using KiotVietTimeSheet.Application.Abstractions;
//using KiotVietTimeSheet.Application.Auth;
//using KiotVietTimeSheet.Application.DomainService.Impls;
//using KiotVietTimeSheet.Application.Dto;
//using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
//using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
//using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
//using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Models;
//using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
//using KiotVietTimeSheet.Domain.AggregatesModels.SettingsAggregate.Models;
//using KiotVietTimeSheet.SharedKernel.Auth;
//using KiotVietTimeSheet.SharedKernel.EventBus;
//using KiotVietTimeSheet.SharedKernel.Specifications;
//using Moq;
//using NUnit.Framework;

//namespace AutoTimeKeepingTest
//{
//    public class AutoTimeKeepingUnitTest
//    {
//        private Mock<IAuthService> authServiceMock;
//        private Mock<ISettingsReadOnlyRepository> settingRepoMock;
//        private Mock<IClockingWriteOnlyRepository> clockingRepoMock;
//        private Mock<IClockingHistoryWriteOnlyRepository> clockingHistoryRepoMock;
//        private Mock<IFingerPrintWriteOnlyRepository> fingerPrintRepoMock;
//        private Mock<ITimeSheetIntegrationEventService> timeSheetIntegrationEventService;
//        private Mock<IFingerMachineWriteOnlyRepository> fingerMachineRepoMock;
//        private const int BranchId = 1;
//        private const int UserId = 2;
//        private const int TenantId = 3;
//        private const int EmployeeId = 4;
//        private const string MachineId = "1234345";
//        private const string FingerCode = "1";

//        [SetUp]
//        public void Setup()
//        {
//            clockingRepoMock = new Mock<IClockingWriteOnlyRepository>();
//            clockingRepoMock
//                .Setup(repo => repo.Update(It.IsAny<Clocking>(), null))
//                .Verifiable();

//            clockingRepoMock
//                .Setup(repo => repo.UnitOfWork.CommitAsync())
//                .Verifiable();

//            timeSheetIntegrationEventService = new Mock<ITimeSheetIntegrationEventService>();
//            timeSheetIntegrationEventService
//                .Setup(x => x.AddEventAsync(It.IsAny<IntegrationEvent>()))
//                .Verifiable();

//            fingerMachineRepoMock = new Mock<IFingerMachineWriteOnlyRepository>();

//            fingerPrintRepoMock = new Mock<IFingerPrintWriteOnlyRepository>();
//            clockingHistoryRepoMock = new Mock<IClockingHistoryWriteOnlyRepository>();
//            settingRepoMock = new Mock<ISettingsReadOnlyRepository>();
//            settingRepoMock
//                .Setup(x => x.GetBySpecification(It.IsAny<ISpecification<Settings>>(), false, false))
//                .Returns(() => new List<Settings>());

//            var context = new ExecutionContext()
//            {
//                BranchId = BranchId,
//                User = new SessionUser() { Id = UserId },
//                TenantId = TenantId
//            };
//            authServiceMock = new Mock<IAuthService>();
//            authServiceMock
//                .Setup(x => x.Context)
//                .Returns(context);
//        }

//        [Test]
//        public async Task FindClockingTestCase1()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 17, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 8, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking1, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase2()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 17, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 2, 8, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, null, "Không tìm thấy ca làm việc phù hợp với dữ liệu chấm công này");
//        }

//        [Test]
//        public async Task FindClockingTestCase3()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 17, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 13, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking2, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase4()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 17, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 6, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking1, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase5()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 17, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 18, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking2, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase6()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 17, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 12, 30, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking1, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase7()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 8, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto() 
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 11, 30, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking1, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase8()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 8, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 10, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 9, 30, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking1, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase9()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.CheckedIn, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 8, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.Created, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 11, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking1, null);
//        }

//        [Test]
//        public async Task FindClockingTestCase10()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 8, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 13, 0, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.CheckedIn, clocking2StartTime, clocking2EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 11, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking2, null);
//        }

//        [Test]
//        public void AutoTimeKeepingTestCase1()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 7, 0, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.Created, clocking1StartTime, clocking1EndTime, null, null, null);

//            var listClocking = new List<Clocking> { clocking1 };

//            var log1 = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 8, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            var log2 = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 11, 0, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123" 
//            };
//            var fingerPrintLogs = new List<FingerPrintLogDto> { log1, log2 };

//            // expected
//            var clockingExpected = clocking1;
//            clockingExpected.CheckInDate = log1.CheckDateTime;
//            clockingExpected.CheckOutDate = log2.CheckDateTime;
//            clockingExpected.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
//            var listClockingExpected = new List<Clocking> { clockingExpected };

//            AutoTimeKeepingTest(fingerPrintLogs, listClocking, listClockingExpected, null);
//        }


//        [Test]
//        public void AutoTimeKeepingTestCase2()
//        {
//            // setup data
//            var clockingStartTime = new DateTime(2019, 10, 1, 10, 0, 0);
//            var clockingEndTime = new DateTime(2019, 10, 1, 15, 0, 0);
//            var clockingCheckedIn = new DateTime(2019, 10, 1, 10, 0, 0);
//            var clockingCheckedOut = new DateTime(2019, 10, 1, 11, 30, 0);
//            var clocking = createClockingTest(ClockingStatuses.CheckedIn, clockingStartTime, clockingEndTime, clockingCheckedIn, clockingCheckedOut, null);
//            var listClocking = new List<Clocking> { clocking };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 11, 45, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            var fingerPrintLogs = new List<FingerPrintLogDto> { log };

//            // expected
//            var clockingExpected = clocking;
//            clockingExpected.CheckOutDate = log.CheckDateTime;
//            clockingExpected.UpdateClockingStatus((byte)ClockingStatuses.CheckedOut);
//            var listClockingExpected = new List<Clocking> { clockingExpected };

//            AutoTimeKeepingTest(fingerPrintLogs, listClocking, listClockingExpected, null);
//        }


//        [Test]
//        public async Task FindClockingTestCase12()
//        {
//            // setup data
//            var clocking1StartTime = new DateTime(2019, 10, 1, 8, 30, 0);
//            var clocking1EndTime = new DateTime(2019, 10, 1, 12, 0, 0);
//            var clocking1 = createClockingTest(ClockingStatuses.WorkOff, clocking1StartTime, clocking1EndTime, null, null, null);

//            var clocking2StartTime = new DateTime(2019, 10, 1, 10, 0, 0);
//            var clocking2EndTime = new DateTime(2019, 10, 1, 15, 0, 0);
//            var clocking2CheckedIn = new DateTime(2019, 10, 1, 10, 0, 0);
//            var clocking2CheckedOut = new DateTime(2019, 10, 1, 11, 30, 0);
//            var clocking2 = createClockingTest(ClockingStatuses.CheckedIn, clocking2StartTime, clocking2EndTime, clocking2CheckedIn, clocking2CheckedOut, null);

//            var listClocking = new List<Clocking> { clocking1, clocking2 };

//            var log = new FingerPrintLogDto()
//            {
//                BranchId = BranchId,
//                EmployeeId = EmployeeId,
//                MachineId = MachineId,
//                FingerCode = FingerCode,
//                CheckDateTime = new DateTime(2019, 10, 1, 11, 45, 0),
//                Status = 0,
//                Employee = null,
//                Uid = "123"
//            };

//            await FindClockingTest(log, listClocking, clocking2, null);
//        }

//        public async Task FindClockingTest(FingerPrintLogDto log, List<Clocking> clockings, Clocking clockingTarget, string failedMsg)
//        {
//            var fingerPrint = new FingerPrint(FingerCode, EmployeeId, "", 1);
//            var listFingerPrint = new List<FingerPrint> { fingerPrint };

//            var fingerMachine = new FingerMachine(BranchId, "machine", FingerMachineVendor.Witeasy, MachineId, "", 1);
//            var listFingerMachine = new List<FingerMachine> { fingerMachine };

//            clockingRepoMock
//                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Clocking>>(), "ClockingHistories"))
//                .Returns(() => Task.FromResult(clockings));

//            fingerPrintRepoMock
//                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<FingerPrint>>(), null))
//                .Returns(() => Task.FromResult(listFingerPrint));

//            fingerPrintRepoMock
//                .Setup(x => x.FindBySpecificationAsync(It.IsAny<ISpecification<FingerPrint>>(), null))
//                .Returns(() => Task.FromResult(fingerPrint));

//            clockingHistoryRepoMock
//                .Setup(x => x.Add(It.IsAny<ClockingHistory>(), null))
//                .Returns((ClockingHistory clockingHistory) => clockingHistory);


//            fingerMachineRepoMock
//                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<FingerMachine>>(), null))
//                .Returns(() => Task.FromResult(listFingerMachine));

//            var settingsObjectDto = new SettingObjectDto(settingRepoMock.Object, authServiceMock.Object);
//            var autoTimeKeepingDomainService = new AutoTimeKeepingDomainService(
//                clockingRepoMock.Object,
//                fingerPrintRepoMock.Object,
//                clockingHistoryRepoMock.Object,
//                settingsObjectDto,
//                timeSheetIntegrationEventService.Object,
//                authServiceMock.Object,
//                fingerMachineRepoMock.Object
//            );
//            try
//            {
//                var findClockingResult = await autoTimeKeepingDomainService.FindClockingForTimeKeepingAsync(log, clockings);
//                Assert.AreSame(clockingTarget, findClockingResult);
//            }
//            catch (Exception e)
//            {
//                Assert.AreEqual(failedMsg, e.Message);
//            }
//        }

//        public void AutoTimeKeepingTest(List<FingerPrintLogDto> logs, List<Clocking> clockings, List<Clocking> expectedClockings, string failedMsg)
//        {
//            var fingerPrint = new FingerPrint(FingerCode, EmployeeId, "", 1);
//            var listFingerPrint = new List<FingerPrint> { fingerPrint };

//            var fingerMachine = new FingerMachine(BranchId, "machine", FingerMachineVendor.Witeasy, MachineId, "", 1);
//            var listFingerMachine = new List<FingerMachine> { fingerMachine };

//            clockingRepoMock
//                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<Clocking>>(), "ClockingHistories"))
//                .Returns(() => Task.FromResult(clockings));

//            fingerPrintRepoMock
//                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<FingerPrint>>(), null))
//                .Returns(() => Task.FromResult(listFingerPrint));

//            fingerPrintRepoMock
//                .Setup(x => x.FindBySpecificationAsync(It.IsAny<ISpecification<FingerPrint>>(), null))
//                .Returns(() => Task.FromResult(fingerPrint));

//            clockingHistoryRepoMock
//                .Setup(x => x.Add(It.IsAny<ClockingHistory>(), null))
//                .Returns((ClockingHistory clockingHistory) => clockingHistory);


//            fingerMachineRepoMock
//                .Setup(x => x.GetBySpecificationAsync(It.IsAny<ISpecification<FingerMachine>>(), null))
//                .Returns(() => Task.FromResult(listFingerMachine));

//            var settingsObjectDto = new SettingObjectDto(settingRepoMock.Object, authServiceMock.Object);
//            var autoTimeKeepingDomainService = new AutoTimeKeepingDomainService(
//                clockingRepoMock.Object,
//                fingerPrintRepoMock.Object,
//                clockingHistoryRepoMock.Object,
//                settingsObjectDto,
//                timeSheetIntegrationEventService.Object,
//                authServiceMock.Object,
//                fingerMachineRepoMock.Object
//            );
//            try
//            {
//                var autoTimeKeepingResults = logs
//                    .Select(async log => await autoTimeKeepingDomainService.SingleAutoTimeKeepingAsync(log, clockings))
//                    .Select(x => x.Result)
//                    .ToList();
//                Assert.IsTrue(clockings.TrueForAll(c =>
//                {
//                    var exist = expectedClockings.FirstOrDefault(x => x.Id == c.Id);
//                    return exist != null && exist.ClockingStatus == c.ClockingStatus &&
//                           exist.CheckInDate == c.CheckInDate && exist.CheckOutDate == c.CheckOutDate;
//                }));
//            }
//            catch (Exception e)
//            {
//                Assert.AreEqual(failedMsg, e.Message);
//            }
//        }

//        private static Clocking createClockingTest(ClockingStatuses status, DateTime starTime, DateTime endTime, DateTime? checkedInDate, DateTime? checkedOutDate, List<ClockingHistory> histories)
//        {
//            return new Clocking(
//                0,
//                0,
//                0,
//                EmployeeId,
//                EmployeeId,
//                (byte)status,
//                starTime,
//                endTime,
//                "",
//                TenantId,
//                BranchId,
//                UserId,
//                DateTime.Now,
//                null,
//                null,
//                false,
//                null,
//                null,
//                checkedInDate,
//                checkedOutDate,
//                0,
//                0,
//                0,
//                0,
//                (byte)AbsenceTypes.AuthorisedAbsence,
//                histories,
//                (byte)ClockingPaymentStatuses.UnPaid,
//                null,
//                null,
//                null
//                );
//        }

//    }
//}