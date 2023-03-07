using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.DomainService;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace KiotVietTimeSheet.Application.Commands.DeleteMultipleEmployee
{
    public class DeleteMultipleEmployeeCommandHandler : BaseCommandHandler,
        IRequestHandler<DeleteMultipleEmployeeCommand, Unit>
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IDeleteEmployeeDomainService _deleteEmployeeDomainService;
        private readonly ILogger<DeleteMultipleEmployeeCommandHandler> _logger;
        private readonly IKiotVietServiceClient _kiotVietServiceClient;
        private readonly IAuthService _authService;
        public DeleteMultipleEmployeeCommandHandler(
            IEventDispatcher eventDispatcher,
            ILogger<DeleteMultipleEmployeeCommandHandler> logger,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IDeleteEmployeeDomainService deleteEmployeeDomainService,
            IKiotVietServiceClient kiotVietServiceClient,
            IAuthService authService
            )
            : base(eventDispatcher)
        {
            _logger = logger;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _deleteEmployeeDomainService = deleteEmployeeDomainService;
            _kiotVietServiceClient = kiotVietServiceClient;
            _authService = authService;
        }

        public async Task<Unit> Handle(DeleteMultipleEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Ids == null || !request.Ids.Any()) return Unit.Value;

                var result = await _deleteEmployeeDomainService.DeleteEmployees(new DomainService.Dto.DeleteEmployeesDomainServiceDto { Ids = request.Ids.ToList() });
                if (!result)
                    return Unit.Value;

                await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
                try
                {
                    foreach (var item in request.Ids)
                    {
                        var employee = new Employee()
                        {
                            Id = item,
                            TenantId = _authService.Context.TenantId
                        };
                        await HandleNotifyChangeAsync(employee);
                    }
                }
                catch (Exception epx)
                {
                    _logger.LogError(epx, epx.Message);
                }

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
                _logger.LogError("[HandleNotifyChange] Delete Employees has empty data");
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
                    _logger.LogInformation($"[HandleNotifyChange] Delete Employees : {log.ToSafeJson()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
