using KiotVietTimeSheet.Application.DomainService.Interfaces;
using KiotVietTimeSheet.Application.Parameters.Interfaces;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;

namespace KiotVietTimeSheet.Application.Parameters.Impls
{
    public class UpdateConfirmClockingParamBuilder : IUpdateConfirmClockingParamBuilder
    {
        private readonly ICalculateTimeClockingDomainService _calculateTimeClockingDomainService;
        private readonly IAutoTimeKeepingDomainService _autoTimeKeepingDomainService;
        private readonly IConfirmClockingDomainService _confirmClockingDomainService;
        private readonly IConfirmClockingWriteOnlyRepository _confirmClockingWriteOnlyRepository;
        private readonly IConfirmClockingHistoryWriteOnlyRepository _confirmClockingHistoryWriteOnlyRepository;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IClockingHistoryWriteOnlyRepository _clockingHistoryWriteOnlyRepository;

        public UpdateConfirmClockingParamBuilder(
            IConfirmClockingWriteOnlyRepository confirmClockingWriteOnlyRepository,
            IConfirmClockingHistoryWriteOnlyRepository confirmClockingHistoryWriteOnlyRepository,
            ICalculateTimeClockingDomainService calculateTimeClockingDomainService,
            IAutoTimeKeepingDomainService autoTimeKeepingDomainService,
            IConfirmClockingDomainService confirmClockingDomainService,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IClockingHistoryWriteOnlyRepository clockingHistoryWriteOnlyRepository
            )
        {
            _calculateTimeClockingDomainService = calculateTimeClockingDomainService;
            _confirmClockingWriteOnlyRepository = confirmClockingWriteOnlyRepository;
            _confirmClockingHistoryWriteOnlyRepository = confirmClockingHistoryWriteOnlyRepository;
            _autoTimeKeepingDomainService = autoTimeKeepingDomainService;
            _confirmClockingDomainService = confirmClockingDomainService;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _clockingHistoryWriteOnlyRepository = clockingHistoryWriteOnlyRepository;
        }

        public IConfirmClockingDomainService GetConfirmClockingDomainService()
        {
            return _confirmClockingDomainService;
        }

        public IAutoTimeKeepingDomainService GetAutoTimeKeepingDomainService()
        {
            return _autoTimeKeepingDomainService;
        }

        public ICalculateTimeClockingDomainService GetCalculateTimeClockingDomainService()
        {
            return _calculateTimeClockingDomainService;
        }

        public IConfirmClockingWriteOnlyRepository GetConfirmClockingWriteOnlyRepository()
        {
            return _confirmClockingWriteOnlyRepository;
        }

        public IConfirmClockingHistoryWriteOnlyRepository GetConfirmClockingHistoryWriteOnlyRepository()
        {
            return _confirmClockingHistoryWriteOnlyRepository;
        }

        public IEmployeeWriteOnlyRepository GetEmployeeWriteOnlyRepository()
        {
            return _employeeWriteOnlyRepository;
        }

        public IClockingHistoryWriteOnlyRepository GetClockingHistoryWriteOnlyRepository()
        {
            return _clockingHistoryWriteOnlyRepository;
        }
    }
}
