using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Service.Interfaces;
using KiotVietTimeSheet.Application.ServiceClients;
using KiotVietTimeSheet.Application.ServiceClients.RequestModels;
using KiotVietTimeSheet.Application.Validators.EmployeeValidators;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.SharedKernel.Domain;
using KiotVietTimeSheet.SharedKernel.Notification;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Application.Commands.AssignUserIdToEmployee
{
    public class AssignUserIdToEmployeeCommandHandler : BaseCommandHandler,
        IRequestHandler<AssignUserIdToEmployeeCommand, Unit>
    {
        private readonly IAuthService _authService;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IPosParamService _posParamService;
        private readonly IKiotVietInternalService _kiotVietInternalService;
        private readonly ILogger<AssignUserIdToEmployeeCommandHandler> _logger;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public AssignUserIdToEmployeeCommandHandler(
            IAuthService authService,
            IEventDispatcher eventDispatcher,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IPosParamService posParamService,
            IKiotVietInternalService kiotVietInternalService,
            ILogger<AssignUserIdToEmployeeCommandHandler> logger,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
            )
            : base(eventDispatcher)
        {
            _authService = authService;
            _eventDispatcher = eventDispatcher;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _posParamService = posParamService;
            _kiotVietInternalService = kiotVietInternalService;
            _logger = logger;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<Unit> Handle(AssignUserIdToEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeDto = request.Employee;
            var employee = await _employeeWriteOnlyRepository.FindByIdAsync(employeeDto.Id);

            if (employee == null)
            {
                await _eventDispatcher.FireEvent(new DomainNotification(typeof(Employee).Name, string.Format(Message.not_exists, Label.employee)));
                return Unit.Value;
            }

            employee.UpdateUserId(employeeDto.UserId);

            Employee employeeHasUserId = null;
            long oldEmployeeIdOfUser = 0;
            if (employee.UserId != null)
            {
                var limitByUserIdSpec =
                    new LimitByUserIdSpec(employee.UserId).Not(new FindEmployeeByIdSpec(employee.Id));
                employeeHasUserId = await _employeeWriteOnlyRepository.FindBySpecificationAsync(limitByUserIdSpec);
                if (employeeHasUserId != null)
                {
                    oldEmployeeIdOfUser = employeeHasUserId.Id;
                }
            }

            var validator = await (new AssignUserIdToEmployeeValidator(_employeeWriteOnlyRepository, _authService, _employeeReadOnlyRepository, request.BlockUnit, oldEmployeeIdOfUser, employee.UserId).ValidateAsync(employee, cancellationToken));
            if (!validator.IsValid)
            {
                NotifyValidationErrors(typeof(Employee), validator.Errors.Select(e => e.ErrorMessage).ToList());
                return Unit.Value;
            }

            _employeeWriteOnlyRepository.Update(employee);
            // Remove relationship of another employee with this user id
            if (employeeHasUserId != null)
            {
                employeeHasUserId.UpdateUserId(null);
                _employeeWriteOnlyRepository.Update(employeeHasUserId);
            }

            await _employeeWriteOnlyRepository.UnitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
