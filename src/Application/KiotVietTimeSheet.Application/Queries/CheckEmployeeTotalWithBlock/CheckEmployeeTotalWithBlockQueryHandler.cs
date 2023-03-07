using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Configuration;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.Utilities.Enums;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Message = KiotVietTimeSheet.Resources.Message;

namespace KiotVietTimeSheet.Application.Queries.CheckEmployeeTotalWithBlock
{
    public class CheckEmployeeTotalWithBlockQueryHandler : QueryHandlerBase,
        IRequestHandler<CheckEmployeeTotalWithBlockQuery, bool>
    {
        private readonly IAuthService _authService;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IApplicationConfiguration _applicationConfiguration;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;
        public CheckEmployeeTotalWithBlockQueryHandler(
            IAuthService authService,
            IEventDispatcher eventDispatcher,
            IApplicationConfiguration applicationConfiguration,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository,
            IPosParamService posParamService
            ) : base(authService)
        {
            _authService = authService;
            _eventDispatcher = eventDispatcher;
            _applicationConfiguration = applicationConfiguration;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<bool> Handle(CheckEmployeeTotalWithBlockQuery request, CancellationToken cancellationToken)
        
        {
            var employeeListPerBlock = _applicationConfiguration.EmployeesPerBlock;
            if (request.BlockUnit <= 0)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(nameof(Employee), new ErrorResult()
                {
                    Code = ErrorCode.RunOutOfQuotaBlockEmployee.ToString(),
                    Message = string.Format(Message.error_whenUpdateData)
                }));
                return false;
            }
            if (employeeListPerBlock > 0)
            {
                var employees = await _employeeReadOnlyRepository.GetBySpecificationAsync(new FindEmployeeByTenantIdSpec(_authService.Context.TenantId));
                employees = employees.Where(x => !x.IsDeleted && x.IsActive).ToList();
                if (employees.Count >= request.BlockUnit * employeeListPerBlock)
                {
                    var packageName = "";
                    if (request.ContractType == (int)KiotVietTimeSheet.Utilities.ContractTypes.Trial
                        || request.ContractType == (int)KiotVietTimeSheet.Utilities.ContractTypes.Basic)
                    {
                        packageName = Label.basic_package;
                    }
                    else if (request.ContractType == (int)KiotVietTimeSheet.Utilities.ContractTypes.Advance)
                    {
                        packageName = Label.advance_package;
                    }
                    
                    await _eventDispatcher.FireEvent(new DomainNotification(nameof(Employee), new ErrorResult()
                    {
                        Code = ErrorCode.RunOutOfQuotaBlockEmployee.ToString(),
                        Message = string.Format(Message.not_runningOutOfQuotaBlockEmployee, packageName, request.BlockUnit * employeeListPerBlock)
                    }));
                    return false;
                }
            }
            return true;
        }

    }
}
