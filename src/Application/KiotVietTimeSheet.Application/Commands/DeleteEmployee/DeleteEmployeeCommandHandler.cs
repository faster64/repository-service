using System;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteEmployeeCommand, Unit>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILogger<DeleteEmployeeCommandHandler> _logger;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IDeleteEmployeeDomainService _deleteEmployeeDomainService;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        public DeleteEmployeeCommandHandler(
            IEventDispatcher eventDispatcher,
            ILogger<DeleteEmployeeCommandHandler> logger,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IDeleteEmployeeDomainService deleteEmployeeDomainService,
            IKiotVietServiceClient kiotVietServiceClient
            )
            : base(eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _logger = logger;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _deleteEmployeeDomainService = deleteEmployeeDomainService;
            _kiotVietServiceClient = kiotVietServiceClient;            
        }

        public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeWriteOnlyRepository.FindByIdAsync(request.Id);

                if (employee == null)
                {
                    await _eventDispatcher.FireEvent(new DomainNotification(typeof(Employee).Name, string.Format(Message.not_exists, Label.employee)));
                    return Unit.Value;
                }

                var result = await _deleteEmployeeDomainService.DeleteEmployee(new DomainService.Dto.DeleteEmployeeDomainServiceDto { Id = employee.Id });
                if (!result)
                {
                    return Unit.Value;
                }

                await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
                await HandleNotifyChangeAsync(employee);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Unit.Value;
            }
        }

        private async Task HandleNotifyChangeAsync(Employee employee)
        {
            if (employee == null || employee.Id <= 0)
            {
                _logger.LogError("[HandleNotifyChange] Delete Employee has empty data");
                return;
            }
            try
            {

                using (var tokenSource = new CancellationTokenSource(Constant.MillisecondsDelay))
                {
                    var req = new OnDelEmployeeReq()
                    {
                        RetailerId = employee.TenantId,
                        EmployeeId = employee.Id
                    };
                    var resp = await _kiotVietServiceClient.OnDeleteEmployee(req, tokenSource.Token);
                    var log = new
                    {
                        Req = req,
                        Message = resp?.ResponseStatus?.Message
                    };
                    _logger.LogInformation($"[HandleNotifyChange] Delete Employee : {log.ToSafeJson()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }

    public class DeleteSyncEmployeeCommandHandler : IRequestHandler<DeleteSyncEmployeeCommand, Unit>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILogger<DeleteSyncEmployeeCommandHandler> _logger;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IDeleteEmployeeDomainService _deleteEmployeeDomainService;
        public DeleteSyncEmployeeCommandHandler(IEventDispatcher eventDispatcher,
            ILogger<DeleteSyncEmployeeCommandHandler> logger,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IDeleteEmployeeDomainService deleteEmployeeDomainService)
        {
            _eventDispatcher = eventDispatcher;
            _logger = logger;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _deleteEmployeeDomainService = deleteEmployeeDomainService;
        }
        public async Task<Unit> Handle(DeleteSyncEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeWriteOnlyRepository.FindByIdWithoutPermission(request.Id);

                if (employee == null)
                {
                    await _eventDispatcher.FireEvent(new DomainNotification(nameof(Employee), string.Format(Message.not_exists, Label.employee)));
                    return Unit.Value;
                }

                var result = await _deleteEmployeeDomainService.DeleteEmployeeWithoutPermission(new DomainService.Dto.DeleteEmployeeDomainServiceDto { Id = employee.Id });
                if (!result)
                {
                    return Unit.Value;
                }

                await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Unit.Value;
            }
        }
    }
}
